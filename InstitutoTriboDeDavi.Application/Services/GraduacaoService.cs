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
    public class GraduacaoService : IGraduacaoService
    {
        private readonly IMapper _mapper;
        private readonly IGraduacaoRepository _repository;
        private readonly IAlunoRepository _alunoRepository;
        private readonly IPoloRepository _poloRepository;
        private readonly IAptidaoGraduacaoRepository _aptidaoRepository;

        public GraduacaoService(
            IMapper mapper,
            IGraduacaoRepository repository,
            IAlunoRepository alunoRepository,
            IPoloRepository poloRepository,
            IAptidaoGraduacaoRepository aptidaoRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _alunoRepository = alunoRepository;
            _poloRepository = poloRepository;
            _aptidaoRepository = aptidaoRepository;
        }

        public Task<List<AptidaoGraduacaoDTO>> ListarAptidao(long? poloId) =>
            _aptidaoRepository.ObterAsync(poloId);

        public async Task<List<GraduacaoDTO>> Listar(int? ano, long? poloId)
        {
            var graduacoes = await _repository.ListarAsync(ano, poloId);
            return await Enriquecer(graduacoes);
        }

        public async Task<List<GraduacaoDTO>> ListarPorAluno(long alunoId)
        {
            var graduacoes = await _repository.ListarPorAlunoAsync(alunoId);
            return await Enriquecer(graduacoes);
        }

        public async Task<GraduacaoDTO> Obter(long id)
        {
            var graduacao = await _repository.ObterAsync(id);
            if (graduacao == null)
                throw new DomainException("Graduação não encontrada.");

            return (await Enriquecer(new List<Graduacao> { graduacao })).First();
        }

        public async Task<ResultadoGraduacaoDTO> Registrar(GraduacaoLoteDTO dto, string registradoPor)
        {
            var pedidos = dto?.Alunos ?? Array.Empty<ItemGraduacaoDTO>();
            if (pedidos.Length == 0)
                throw new DomainException("Selecione ao menos um aluno.");

            var alunos = await _alunoRepository.GetAllAsync();
            var porId = alunos.ToDictionary(a => a.Id);

            var itens = new List<(Graduacao, Aluno)>();
            var ignorados = new List<string>();

            foreach (var pedido in pedidos)
            {
                if (!porId.TryGetValue(pedido.AlunoId, out var aluno))
                {
                    ignorados.Add($"Aluno #{pedido.AlunoId}: não encontrado.");
                    continue;
                }

                var faixaAtual = (int)aluno.Faixa;

                // Um aluno já promovido não é rebaixado nem regraduado por
                // engano: em vez de falhar o lote inteiro, ele é só ignorado e
                // o motivo volta para quem registrou.
                if (pedido.FaixaNova <= faixaAtual)
                {
                    ignorados.Add($"{aluno.Nome}: já está na faixa {faixaAtual} ou superior.");
                    continue;
                }

                var graduacao = new Graduacao
                {
                    AlunoId = aluno.Id,
                    PoloId = aluno.PoloId,
                    FaixaAnterior = faixaAtual,
                    FaixaNova = pedido.FaixaNova,
                    Data = dto.Data,
                    Observacao = dto.Observacao ?? string.Empty,
                    RegistradoPor = registradoPor ?? string.Empty,
                };
                graduacao.Validate();

                itens.Add((graduacao, aluno));
            }

            if (itens.Count == 0)
                throw new DomainException(
                    "Nenhum aluno pôde ser graduado. " + string.Join(" ", ignorados));

            var total = await _repository.RegistrarAsync(itens);

            return new ResultadoGraduacaoDTO
            {
                Graduados = total,
                Ignorados = ignorados.ToArray(),
                Mensagem = ignorados.Count == 0
                    ? $"{total} aluno(s) graduado(s)!"
                    : $"{total} aluno(s) graduado(s); {ignorados.Count} ignorado(s).",
            };
        }

        public async Task Excluir(long id)
        {
            var graduacao = await _repository.ObterAsync(id);
            if (graduacao == null)
                throw new DomainException("Graduação não encontrada.");

            var aluno = await _alunoRepository.GetByIdAsync(graduacao.AlunoId);
            await _repository.ExcluirAsync(graduacao, aluno);
        }

        // Junta nome do aluno e do polo — a tela mostra a lista sem cruzar dados.
        private async Task<List<GraduacaoDTO>> Enriquecer(List<Graduacao> graduacoes)
        {
            var dtos = _mapper.Map<List<GraduacaoDTO>>(graduacoes);
            if (dtos.Count == 0) return dtos;

            var alunos = await _alunoRepository.GetAllAsync();
            var nomeAluno = alunos.ToDictionary(a => a.Id, a => a.Nome);

            var polos = await _poloRepository.GetAllAsync();
            var nomePolo = polos.ToDictionary(p => p.Id, p => p.Nome);

            foreach (var d in dtos)
            {
                d.NomeAluno = nomeAluno.TryGetValue(d.AlunoId, out var n) ? n : $"Aluno #{d.AlunoId}";
                d.PoloNome = nomePolo.TryGetValue(d.PoloId, out var p) ? p : "-";
            }

            return dtos;
        }
    }
}
