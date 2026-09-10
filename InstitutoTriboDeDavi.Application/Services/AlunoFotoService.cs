using System;
using System.IO;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Common;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class AlunoFotoService : IAlunoFotoService
    {
        private readonly IAlunoRepository _repository;
        private readonly IFotoStorage _storage;
        private readonly IFotoArquivoRepository _fotoArquivoRepository;

        public AlunoFotoService(
            IAlunoRepository repository,
            IFotoStorage storage,
            IFotoArquivoRepository fotoArquivoRepository)
        {
            _repository = repository;
            _storage = storage;
            _fotoArquivoRepository = fotoArquivoRepository;
        }

        public async Task SalvarFoto(long alunoId, string nomeArquivo, string contentType, Stream conteudo)
        {
            var aluno = await _repository.GetByIdAsync(alunoId);
            if (aluno == null) throw new DomainException("Não existe um aluno com o ID informado!");
            if (conteudo == null || conteudo.Length == 0)
                throw new DomainException("Nenhuma imagem enviada.");

            var novoArquivoId = await _storage.UploadAsync(nomeArquivo, contentType, conteudo);
            var antigo = await _repository.DefinirFotoAsync(alunoId, novoArquivoId);
            if (!string.IsNullOrEmpty(antigo))
                await _storage.ExcluirAsync(antigo);
        }

        public async Task RemoverFoto(long alunoId)
        {
            var antigo = await _repository.DefinirFotoAsync(alunoId, null);
            if (!string.IsNullOrEmpty(antigo))
                await _storage.ExcluirAsync(antigo);
        }

        public async Task<string> ObterFotoDataUri(long alunoId, bool mini = false)
        {
            var aluno = await _repository.GetByIdAsync(alunoId);
            if (aluno == null || string.IsNullOrEmpty(aluno.FotoArquivoId)) return null;

            if (mini)
            {
                var uri = await ObterMiniaturaDataUri(aluno.FotoArquivoId);
                if (uri != null) return uri; // se não deu, cai para a foto cheia
            }

            var download = await _storage.BaixarAsync(aluno.FotoArquivoId);
            if (download == null) return null;

            using var ms = new MemoryStream();
            await download.Conteudo.CopyToAsync(ms);
            var base64 = Convert.ToBase64String(ms.ToArray());
            return $"data:{download.ContentType};base64,{base64}";
        }

        // Miniatura (avatar/grade): devolve a guardada ou gera na hora a partir
        // da foto cheia e guarda para as próximas — assim as fotos já existentes
        // também passam a ter miniatura sem migração de dados. Null se não der
        // (foto não é do storage do banco, ou imagem inválida) → usa a cheia.
        private async Task<string> ObterMiniaturaDataUri(string fotoArquivoId)
        {
            if (!long.TryParse(fotoArquivoId, out var id)) return null;

            var arquivo = await _fotoArquivoRepository.ObterAsync(id);
            if (arquivo?.Conteudo == null) return null;

            var miniatura = arquivo.Miniatura;
            if (miniatura == null || miniatura.Length == 0)
            {
                try
                {
                    miniatura = Imagem.GerarMiniatura(arquivo.Conteudo, 128);
                }
                catch
                {
                    return null; // imagem inválida/formato não suportado
                }
                await _fotoArquivoRepository.SalvarMiniaturaAsync(id, miniatura);
            }

            var base64 = Convert.ToBase64String(miniatura);
            return $"data:image/jpeg;base64,{base64}";
        }

        public async Task<long?> ObterPoloId(long alunoId)
        {
            var aluno = await _repository.GetByIdAsync(alunoId);
            return aluno?.PoloId;
        }

        public async Task<ConfiguracaoFotoAlunoDTO> ObterConfig()
        {
            var cfg = await _repository.ObterConfigFotoAsync() ?? new ConfiguracaoFotoAluno();
            return new ConfiguracaoFotoAlunoDTO
            {
                MostrarNoCadastro = cfg.MostrarNoCadastro,
                MostrarNaChamada = cfg.MostrarNaChamada,
                MostrarNoResponsavel = cfg.MostrarNoResponsavel,
                MostrarNaCarteirinha = cfg.MostrarNaCarteirinha,
            };
        }

        public Task SalvarConfig(ConfiguracaoFotoAlunoDTO dto)
        {
            return _repository.SalvarConfigFotoAsync(new ConfiguracaoFotoAluno
            {
                MostrarNoCadastro = dto.MostrarNoCadastro,
                MostrarNaChamada = dto.MostrarNaChamada,
                MostrarNoResponsavel = dto.MostrarNoResponsavel,
                MostrarNaCarteirinha = dto.MostrarNaCarteirinha,
            });
        }
    }
}
