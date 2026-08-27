using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class MensalidadesService : IMensalidadesService
    {
        private readonly IMapper _mapper;
        private readonly IMensalidadesRepository _repository;

        public MensalidadesService(IMapper mapper, IMensalidadesRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        // ── Planos ────────────────────────────────────────────────────────
        public async Task<List<PlanoMensalidadeDTO>> ListarPlanos()
        {
            var planos = await _repository.ListarPlanosAsync();
            return _mapper.Map<List<PlanoMensalidadeDTO>>(planos);
        }

        public async Task<PlanoMensalidadeDTO> SalvarPlano(PlanoMensalidadeDTO dto)
        {
            var plano = _mapper.Map<PlanoMensalidade>(dto);
            plano.Validate();

            var salvo = await _repository.SalvarPlanoAsync(plano);
            if (salvo == null)
                throw new DomainException("Não existe um plano com o ID informado!");

            return _mapper.Map<PlanoMensalidadeDTO>(salvo);
        }

        public Task ExcluirPlano(long id) => _repository.ExcluirPlanoAsync(id);

        // ── Matrículas ────────────────────────────────────────────────────
        public async Task<List<MatriculaFinanceiraDTO>> ListarMatriculas()
        {
            var matriculas = await _repository.ListarMatriculasAsync();
            return _mapper.Map<List<MatriculaFinanceiraDTO>>(matriculas);
        }

        public async Task<MatriculaFinanceiraDTO> SalvarMatricula(MatriculaFinanceiraDTO dto)
        {
            var matricula = _mapper.Map<MatriculaFinanceira>(dto);
            matricula.Validate();

            if (await _repository.ObterPlanoAsync(matricula.PlanoId) == null)
                throw new DomainException("O plano informado não existe!");

            var salva = await _repository.SalvarMatriculaAsync(matricula);
            if (salva == null)
                throw new DomainException("Não existe uma matrícula com o ID informado!");

            return _mapper.Map<MatriculaFinanceiraDTO>(salva);
        }

        public Task ExcluirMatricula(long id) => _repository.ExcluirMatriculaAsync(id);

        // ── Cobranças ─────────────────────────────────────────────────────
        public async Task<List<CobrancaDTO>> ListarCobrancas(string competencia)
        {
            var cobrancas = await _repository.ListarCobrancasAsync(competencia);
            return _mapper.Map<List<CobrancaDTO>>(cobrancas);
        }

        public async Task<ResultadoGeracaoDTO> GerarCobrancas(string competencia)
        {
            if (string.IsNullOrWhiteSpace(competencia) || competencia.Length != 7 || competencia[4] != '-')
                throw new DomainException("Competência inválida (esperado \"yyyy-MM\").");

            var matriculas = await _repository.ListarMatriculasAtivasAsync();
            var planos = (await _repository.ListarPlanosAsync()).ToDictionary(p => p.Id);
            var jaTem = await _repository.AlunosComCobrancaAsync(competencia);

            var novas = new List<Cobranca>();
            var ignoradas = 0;

            foreach (var m in matriculas)
            {
                // Já tem cobrança na competência, ainda não começou, ou plano
                // inexistente/inativo → não gera.
                if (jaTem.Contains(m.AlunoId)) { ignoradas++; continue; }
                if (!string.IsNullOrEmpty(m.Inicio) && string.CompareOrdinal(m.Inicio, competencia) > 0)
                {
                    ignoradas++;
                    continue;
                }
                if (!planos.TryGetValue(m.PlanoId, out var plano) || !plano.Ativo)
                {
                    ignoradas++;
                    continue;
                }

                var isento = m.DescontoTipo == "isencao";
                novas.Add(new Cobranca
                {
                    AlunoId = m.AlunoId,
                    PlanoId = plano.Id,
                    Competencia = competencia,
                    Vencimento = VencimentoDe(competencia, m.DiaVencimento),
                    Valor = ValorComDesconto(plano.Valor, m.DescontoTipo, m.DescontoValor),
                    Status = isento ? "isento" : "pendente",
                    PagamentoForma = string.Empty,
                    Observacao = string.Empty,
                });
            }

            var geradas = await _repository.AdicionarCobrancasAsync(novas);

            return new ResultadoGeracaoDTO
            {
                Geradas = geradas,
                Ignoradas = ignoradas,
                Mensagem = $"{geradas} cobrança(s) gerada(s), {ignoradas} ignorada(s).",
            };
        }

        public async Task<CobrancaDTO> Baixar(BaixaCobrancaDTO dto)
        {
            if (dto.ContaId <= 0)
                throw new DomainException("Informe a conta que recebeu o pagamento.");
            if (dto.PagamentoValor <= 0)
                throw new DomainException("O valor do pagamento deve ser maior que zero.");

            var cobranca = await _repository.BaixarAsync(
                dto.Id, dto.PagamentoData, dto.PagamentoValor,
                dto.PagamentoForma ?? string.Empty, dto.ContaId);

            if (cobranca == null)
                throw new DomainException("Não existe uma cobrança com o ID informado!");

            return _mapper.Map<CobrancaDTO>(cobranca);
        }

        public async Task<CobrancaDTO> SalvarCobranca(CobrancaDTO dto)
        {
            var cobranca = _mapper.Map<Cobranca>(dto);
            cobranca.Validate();

            var salva = await _repository.SalvarCobrancaAsync(cobranca);
            if (salva == null)
                throw new DomainException("Não existe uma cobrança com o ID informado!");

            return _mapper.Map<CobrancaDTO>(salva);
        }

        public Task ExcluirCobranca(long id) => _repository.ExcluirCobrancaAsync(id);

        // ── Regras ────────────────────────────────────────────────────────

        // Valor a cobrar aplicando o desconto da matrícula (nunca negativo).
        private static decimal ValorComDesconto(decimal valor, string tipo, decimal desconto)
        {
            switch (tipo)
            {
                case "isencao":
                    return 0m;
                case "percentual":
                    var pct = Math.Min(100m, Math.Max(0m, desconto));
                    return Math.Max(0m, Math.Round(valor * (1 - pct / 100m), 2));
                case "valor":
                    return Math.Max(0m, Math.Round(valor - Math.Max(0m, desconto), 2));
                default:
                    return Math.Max(0m, valor);
            }
        }

        // Data de vencimento na competência, respeitando o último dia do mês.
        private static DateTime VencimentoDe(string competencia, int dia)
        {
            var partes = competencia.Split('-');
            var ano = int.Parse(partes[0]);
            var mes = int.Parse(partes[1]);
            var ultimoDia = DateTime.DaysInMonth(ano, mes);
            var d = Math.Min(Math.Max(1, dia), ultimoDia);
            return new DateTime(ano, mes, d);
        }
    }
}
