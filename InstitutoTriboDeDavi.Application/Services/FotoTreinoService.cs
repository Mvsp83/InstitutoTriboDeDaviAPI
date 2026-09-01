using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class FotoTreinoService : IFotoTreinoService
    {
        private readonly IFotoTreinoRepository _repository;
        private readonly IFotoStorage _storage;

        public FotoTreinoService(IFotoTreinoRepository repository, IFotoStorage storage)
        {
            _repository = repository;
            _storage = storage;
        }

        // Categorias válidas do álbum público.
        private static readonly string[] Categorias = { "polo", "graduacoes", "geral", "eventos" };

        public async Task<FotoTreinoDTO> Postar(
            string categoria, long poloId, int turma, DateTime dataAula, string legenda,
            long professorId, string nomeArquivo, string contentType, Stream conteudo,
            bool consentimento)
        {
            // LGPD: sem a confirmação de autorização de imagem, não publica.
            if (!consentimento)
                throw new DomainException("Confirme a autorização de uso de imagem das pessoas na foto (LGPD).");

            categoria = string.IsNullOrWhiteSpace(categoria) ? "polo" : categoria.Trim().ToLowerInvariant();
            if (!Categorias.Contains(categoria))
                throw new DomainException("Categoria inválida.");
            if (dataAula == default) throw new DomainException("Informe a data da foto.");
            if (conteudo == null || conteudo.Length == 0)
                throw new DomainException("Nenhuma imagem enviada.");

            var dia = dataAula.Date;
            var ehPolo = categoria == "polo";

            if (ehPolo)
            {
                if (poloId <= 0) throw new DomainException("Polo inválido.");
                if (turma <= 0) throw new DomainException("Informe a turma.");
            }

            // Treino de polo: respeita a moderação por polo. Coleções do admin
            // (graduações/geral/eventos) já entram publicadas (o admin é confiável).
            bool jaPublica;
            if (ehPolo)
            {
                var config = await _repository.ObterConfigAsync(poloId);
                jaPublica = config != null && !config.RequerAutorizacao;
            }
            else
            {
                jaPublica = true;
            }

            // Sobe a nova imagem PRIMEIRO: se falhar, não mexemos na atual.
            var novoArquivoId = await _storage.UploadAsync(nomeArquivo, contentType, conteudo);

            // Só a categoria "polo" tem a trava 1/turma/aula (substitui a existente).
            if (ehPolo)
            {
                var existente = await _repository.ObterPorAulaAsync(poloId, turma, dia);
                if (existente != null)
                {
                    var antigoArquivoId = existente.ArquivoId;
                    existente.ArquivoId = novoArquivoId;
                    existente.Legenda = legenda ?? string.Empty;
                    existente.ProfessorId = professorId;
                    existente.DataAula = dia;
                    existente.Publicada = jaPublica;
                    existente.ConsentimentoConfirmado = consentimento;
                    existente.Validate();

                    var atualizada = await _repository.AtualizarAsync(existente);
                    await _storage.ExcluirAsync(antigoArquivoId);
                    return MapDto(atualizada, null);
                }
            }

            var foto = new FotoTreino
            {
                Categoria = categoria,
                PoloId = ehPolo ? poloId : 0,
                Turma = ehPolo ? turma : 0,
                DataAula = dia,
                Legenda = legenda ?? string.Empty,
                ArquivoId = novoArquivoId,
                ProfessorId = professorId,
                Publicada = jaPublica,
                CriadoEm = DateTime.Now,
                ConsentimentoConfirmado = consentimento,
            };
            foto.Validate();

            var criada = await _repository.AdicionarAsync(foto);
            return MapDto(criada, null);
        }

        public async Task<List<FotoTreinoDTO>> Listar()
        {
            var itens = await _repository.ListarTodasAsync();
            return itens.Select(x => MapDto(x.foto, x.poloNome)).ToList();
        }

        public async Task<List<FotoTreinoPublicaDTO>> ListarPublicas()
        {
            var itens = await _repository.ListarPublicasAsync();
            return itens.Select(x => new FotoTreinoPublicaDTO
            {
                Id = x.foto.Id,
                Categoria = x.foto.Categoria,
                PoloNome = x.poloNome,
                Turma = x.foto.Turma,
                DataAula = x.foto.DataAula,
                Legenda = x.foto.Legenda,
                Url = UrlArquivo(x.foto.Id),
            }).ToList();
        }

        public Task DefinirPublicacao(long id, bool publicada) =>
            _repository.DefinirPublicacaoAsync(id, publicada);

        public async Task Excluir(long id)
        {
            var foto = await _repository.ObterAsync(id);
            if (foto == null) return;
            await _repository.ExcluirAsync(id);
            await _storage.ExcluirAsync(foto.ArquivoId);
        }

        public async Task<FotoTreinoDTO> Obter(long id)
        {
            var foto = await _repository.ObterAsync(id);
            return foto == null ? null : MapDto(foto, null);
        }

        public async Task<FotoDownload> BaixarArquivo(long id)
        {
            var foto = await _repository.ObterAsync(id);
            if (foto == null) return null;
            return await _storage.BaixarAsync(foto.ArquivoId);
        }

        public async Task<string> ObterPreviaDataUri(long id)
        {
            var download = await BaixarArquivo(id);
            if (download == null) return null;

            using var ms = new MemoryStream();
            await download.Conteudo.CopyToAsync(ms);
            var base64 = Convert.ToBase64String(ms.ToArray());
            return $"data:{download.ContentType};base64,{base64}";
        }

        // ── Config por polo ───────────────────────────────────────────────
        public async Task<List<PoloFotoConfigDTO>> ListarConfigPolos()
        {
            var itens = await _repository.ListarConfigPolosAsync();
            return itens.Select(x => new PoloFotoConfigDTO
            {
                PoloId = x.poloId,
                PoloNome = x.poloNome,
                RequerAutorizacao = x.requerAutorizacao,
            }).ToList();
        }

        public Task DefinirConfigPolo(long poloId, bool requerAutorizacao) =>
            _repository.DefinirConfigAsync(poloId, requerAutorizacao);

        // ── Auxiliares ────────────────────────────────────────────────────
        private static string UrlArquivo(long id) => $"/api/FotosTreino/{id}/arquivo";

        private static FotoTreinoDTO MapDto(FotoTreino f, string poloNome) => new()
        {
            Id = f.Id,
            Categoria = f.Categoria,
            PoloId = f.PoloId,
            PoloNome = poloNome,
            Turma = f.Turma,
            DataAula = f.DataAula,
            Legenda = f.Legenda,
            ProfessorId = f.ProfessorId,
            Publicada = f.Publicada,
            CriadoEm = f.CriadoEm,
            Url = UrlArquivo(f.Id),
        };
    }
}
