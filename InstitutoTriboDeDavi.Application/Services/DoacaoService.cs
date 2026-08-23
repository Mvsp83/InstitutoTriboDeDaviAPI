using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class DoacaoService : IDoacaoService
    {
        // Tipo do documento oficial usado no recibo de doação. Ofício = 0 e
        // Recibo = 1 já existiam; o tipo próprio dá ao recibo de doação uma
        // sequência de numeração separada, como manda a prestação de contas.
        private const int TipoReciboDoacao = 2;

        private readonly IMapper _mapper;
        private readonly IDoacaoRepository _repository;
        private readonly IDocumentoOficialService _documentos;

        public DoacaoService(
            IMapper mapper,
            IDoacaoRepository repository,
            IDocumentoOficialService documentos)
        {
            _mapper = mapper;
            _repository = repository;
            _documentos = documentos;
        }

        // ── Doadores ──────────────────────────────────────────────────────
        public async Task<List<DoadorDTO>> ListarDoadores()
        {
            var doadores = await _repository.ListarDoadoresAsync();
            var dtos = _mapper.Map<List<DoadorDTO>>(doadores);

            // Resumo por doador: total, quantidade e data da última doação.
            var doacoes = await _repository.ListarDoacoesAsync(null, null);
            var porDoador = doacoes
                .Where(d => d.DoadorId.HasValue)
                .GroupBy(d => d.DoadorId.Value)
                .ToDictionary(
                    g => g.Key,
                    g => (
                        total: g.Sum(x => x.Valor),
                        qtd: g.Count(),
                        ultima: g.Max(x => x.Data)
                    ));

            foreach (var d in dtos)
            {
                if (!porDoador.TryGetValue(d.Id, out var r)) continue;
                d.TotalDoado = r.total;
                d.QuantidadeDoacoes = r.qtd;
                d.UltimaDoacao = r.ultima;
            }

            return dtos;
        }

        public async Task<DoadorDTO> SalvarDoador(DoadorDTO dto)
        {
            var doador = _mapper.Map<Doador>(dto);
            doador.Validate();

            var salvo = await _repository.SalvarDoadorAsync(doador);
            if (salvo == null)
                throw new DomainException("Não existe um doador com o ID informado!");

            return _mapper.Map<DoadorDTO>(salvo);
        }

        public async Task ExcluirDoador(long id)
        {
            var ok = await _repository.ExcluirDoadorAsync(id);
            if (!ok)
                throw new DomainException(
                    "Este doador tem doações registradas e não pode ser excluído. " +
                    "Marque-o como inativo se não quiser mais vê-lo na lista.");
        }

        // ── Doações ───────────────────────────────────────────────────────
        public async Task<List<DoacaoDTO>> ListarDoacoes(int? ano, long? doadorId)
        {
            var doacoes = await _repository.ListarDoacoesAsync(ano, doadorId);
            var dtos = _mapper.Map<List<DoacaoDTO>>(doacoes);

            var doadores = await _repository.ListarDoadoresAsync();
            var nomes = doadores.ToDictionary(d => d.Id, d => d.Nome);

            foreach (var d in dtos)
                d.NomeDoador = d.DoadorId.HasValue && nomes.TryGetValue(d.DoadorId.Value, out var n)
                    ? n
                    : "Anônimo";

            return dtos;
        }

        public async Task<DoacaoDTO> SalvarDoacao(DoacaoDTO dto, string registradoPor)
        {
            var doacao = _mapper.Map<Doacao>(dto);
            doacao.RegistradoPor = registradoPor ?? string.Empty;
            doacao.Validate();

            if (doacao.DoadorId.HasValue &&
                await _repository.ObterDoadorAsync(doacao.DoadorId.Value) == null)
                throw new DomainException("O doador informado não existe.");

            var salva = await _repository.SalvarDoacaoAsync(doacao);
            if (salva == null)
                throw new DomainException("Não existe uma doação com o ID informado!");

            return _mapper.Map<DoacaoDTO>(salva);
        }

        public Task ExcluirDoacao(long id) => _repository.ExcluirDoacaoAsync(id);

        public async Task<ResumoDoacoesDTO> Resumo(int ano)
        {
            var doacoes = await _repository.ListarDoacoesAsync(ano, null);
            var total = doacoes.Sum(d => d.Valor);

            return new ResumoDoacoesDTO
            {
                Ano = ano,
                Total = total,
                Quantidade = doacoes.Count,
                Doadores = doacoes.Where(d => d.DoadorId.HasValue)
                                  .Select(d => d.DoadorId.Value)
                                  .Distinct()
                                  .Count(),
                TicketMedio = doacoes.Count > 0 ? Math.Round(total / doacoes.Count, 2) : 0,
            };
        }

        // ── Recibo ────────────────────────────────────────────────────────
        public async Task<DoacaoDTO> EmitirRecibo(long doacaoId)
        {
            var doacao = await _repository.ObterDoacaoAsync(doacaoId);
            if (doacao == null)
                throw new DomainException("Doação não encontrada.");

            if (doacao.ReciboDocumentoId.HasValue)
                throw new DomainException(
                    $"Esta doação já tem o recibo {doacao.ReciboNumero} emitido.");

            // Recibo de doação identifica quem doou — é o documento que o doador
            // usa para comprovar a doação.
            if (!doacao.DoadorId.HasValue)
                throw new DomainException(
                    "Doação anônima não gera recibo. Vincule um doador antes de emitir.");

            var doador = await _repository.ObterDoadorAsync(doacao.DoadorId.Value);
            if (doador == null)
                throw new DomainException("O doador desta doação não existe mais.");

            // O conteúdo vai como JSON, no mesmo formato dos demais documentos
            // oficiais — assim o recibo também aparece em Ofícios e Recibos.
            var conteudo = JsonSerializer.Serialize(new
            {
                doadorNome = doador.Nome,
                doadorDocumento = doador.Documento,
                doadorEndereco = doador.Endereco,
                doadorCidade = doador.Cidade,
                valor = doacao.Valor,
                data = doacao.Data,
                forma = doacao.Forma,
                finalidade = doacao.Finalidade,
            });

            var documento = await _documentos.Create(new DocumentoOficialDTO
            {
                Tipo = TipoReciboDoacao,
                DataDocumento = doacao.Data,
                Titulo = $"Recibo de doação — {doador.Nome}",
                Conteudo = conteudo,
            });

            // A numeração oficial só é atribuída na aprovação. A doação já
            // aconteceu, então o recibo nasce aprovado — e imutável.
            var aprovado = await _documentos.Aprovar(documento.Id);

            // Grava o vínculo por método dedicado — SalvarDoacaoAsync ignora
            // os campos de recibo (para a edição da doação não os alterar), então
            // usá-lo aqui deixava a doação SEM recibo registrado e a trava
            // anti-duplicação nunca disparava.
            await _repository.VincularReciboAsync(doacao.Id, aprovado.Id, aprovado.NumeroFormatado);
            doacao.ReciboDocumentoId = aprovado.Id;
            doacao.ReciboNumero = aprovado.NumeroFormatado;

            return _mapper.Map<DoacaoDTO>(doacao);
        }
    }
}
