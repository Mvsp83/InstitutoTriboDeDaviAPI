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
    public class FinanceiroService : IFinanceiroService
    {
        private readonly IMapper _mapper;
        private readonly IFinanceiroRepository _repository;

        public FinanceiroService(IMapper mapper, IFinanceiroRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        // ── Contas ────────────────────────────────────────────────────────
        public async Task<List<ContaFinanceiraDTO>> ListarContas()
        {
            var contas = await _repository.ListarContasAsync();
            return _mapper.Map<List<ContaFinanceiraDTO>>(contas);
        }

        public async Task<ContaFinanceiraDTO> SalvarConta(ContaFinanceiraDTO dto)
        {
            var conta = _mapper.Map<ContaFinanceira>(dto);
            conta.Validate();

            var salva = await _repository.SalvarContaAsync(conta);
            if (salva == null)
                throw new DomainException("Não existe uma conta com o ID informado!");

            return _mapper.Map<ContaFinanceiraDTO>(salva);
        }

        public Task ExcluirConta(long id) => _repository.ExcluirContaAsync(id);

        // ── Movimentações ─────────────────────────────────────────────────
        public async Task<List<MovimentacaoFinanceiraDTO>> ListarMovimentacoes()
        {
            var movs = await _repository.ListarMovimentacoesAsync();
            return _mapper.Map<List<MovimentacaoFinanceiraDTO>>(movs);
        }

        public async Task<MovimentacaoFinanceiraDTO> SalvarMovimentacao(MovimentacaoFinanceiraDTO dto)
        {
            var mov = _mapper.Map<MovimentacaoFinanceira>(dto);
            mov.Validate();

            if (await _repository.ObterContaAsync(mov.ContaId) == null)
                throw new DomainException("A conta informada não existe!");

            // Lançamento de transferência só é criado pelo endpoint próprio,
            // que garante os dois lados juntos.
            if (dto.Id == 0 && !string.IsNullOrEmpty(mov.TransferenciaId))
                throw new DomainException("Use o registro de transferência para lançamentos entre contas.");

            var salva = await _repository.SalvarMovimentacaoAsync(mov);
            if (salva == null)
                throw new DomainException("Não existe um lançamento com o ID informado!");

            return _mapper.Map<MovimentacaoFinanceiraDTO>(salva);
        }

        public Task ExcluirMovimentacao(long id) => _repository.ExcluirMovimentacaoAsync(id);

        public Task DefinirConciliacao(long id, bool conciliado) =>
            _repository.DefinirConciliacaoAsync(id, conciliado);

        public async Task RegistrarTransferencia(TransferenciaDTO dto)
        {
            if (dto.ContaOrigemId == dto.ContaDestinoId)
                throw new DomainException("A conta de origem e a de destino devem ser diferentes.");

            if (dto.Valor <= 0)
                throw new DomainException("O valor da transferência deve ser maior que zero.");

            if (await _repository.ObterContaAsync(dto.ContaOrigemId) == null)
                throw new DomainException("A conta de origem não existe!");

            if (await _repository.ObterContaAsync(dto.ContaDestinoId) == null)
                throw new DomainException("A conta de destino não existe!");

            // Os dois lados compartilham o mesmo identificador para que possam
            // ser reconhecidos (e excluídos) como um par.
            var transferenciaId = Guid.NewGuid().ToString();

            var debito = new MovimentacaoFinanceira
            {
                ContaId = dto.ContaOrigemId,
                Data = dto.Data,
                Descricao = dto.Descricao ?? string.Empty,
                CategoriaId = dto.CategoriaId,
                Tipo = "Debito",
                Valor = dto.Valor,
                Conciliado = false,
                Documento = dto.Documento ?? string.Empty,
                Observacoes = dto.Observacoes ?? string.Empty,
                TransferenciaId = transferenciaId,
            };

            var credito = new MovimentacaoFinanceira
            {
                ContaId = dto.ContaDestinoId,
                Data = dto.Data,
                Descricao = dto.Descricao ?? string.Empty,
                CategoriaId = dto.CategoriaId,
                Tipo = "Credito",
                Valor = dto.Valor,
                Conciliado = false,
                Documento = dto.Documento ?? string.Empty,
                Observacoes = dto.Observacoes ?? string.Empty,
                TransferenciaId = transferenciaId,
            };

            debito.Validate();
            credito.Validate();

            await _repository.RegistrarTransferenciaAsync(debito, credito);
        }

        // ── Importação do localStorage ────────────────────────────────────
        public async Task<ResultadoImportacaoDTO> Importar(ImportacaoFinanceiraDTO dto)
        {
            // Só aceita a carga com o financeiro ainda vazio: importar por cima
            // de dados existentes duplicaria lançamentos.
            if (await _repository.ExisteAlgumDadoAsync())
                throw new DomainException(
                    "Já existem dados financeiros no servidor. A importação só é permitida uma vez, com o financeiro vazio.");

            var contas = (dto.Contas ?? Array.Empty<ContaFinanceiraDTO>())
                .Select(c =>
                {
                    var entidade = _mapper.Map<ContaFinanceira>(c);
                    entidade.Validate();
                    return (conta: entidade, idOrigem: c.Id);
                })
                .ToList();

            var movs = (dto.Movimentacoes ?? Array.Empty<MovimentacaoFinanceiraDTO>())
                .Select(m =>
                {
                    var entidade = _mapper.Map<MovimentacaoFinanceira>(m);
                    entidade.Validate();
                    return (mov: entidade, contaIdOrigem: m.ContaId);
                })
                .ToList();

            if (contas.Count == 0 && movs.Count == 0)
                throw new DomainException("Não há dados para importar.");

            var (qtdContas, qtdMovs) = await _repository.ImportarAsync(contas, movs);

            return new ResultadoImportacaoDTO
            {
                ContasImportadas = qtdContas,
                MovimentacoesImportadas = qtdMovs,
                Mensagem = $"{qtdContas} conta(s) e {qtdMovs} lançamento(s) importados.",
            };
        }
    }
}
