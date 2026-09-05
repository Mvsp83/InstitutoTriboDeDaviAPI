using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IMapper _mapper;
        private readonly IProdutoRepository _repository;

        public ProdutoService(IMapper mapper, IProdutoRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<List<ProdutoDTO>> Listar() =>
            _mapper.Map<List<ProdutoDTO>>(await _repository.ListarTodosAsync());

        // Projeção pública: esconde o estoque exato (só "disponível") e omite
        // Ativo/DataCriacao. A vitrine já traz apenas produtos ativos.
        public async Task<List<ProdutoVitrineDTO>> Vitrine()
        {
            var produtos = await _repository.ListarVitrineAsync();
            return produtos.Select(p => new ProdutoVitrineDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Preco = p.Preco,
                FotoArquivoId = p.FotoArquivoId,
                TemFoto = !string.IsNullOrEmpty(p.FotoArquivoId),
                FormasPagamento = p.FormasPagamento,
                Informacoes = p.Informacoes,
                Variacoes = p.Variacoes.Select(v => new VariacaoVitrineDTO
                {
                    Id = v.Id,
                    Tamanho = v.Tamanho,
                    Cor = v.Cor,
                    Disponivel = v.Quantidade > 0,
                }).ToList(),
            }).ToList();
        }

        public async Task<ProdutoDTO> Obter(long id) =>
            _mapper.Map<ProdutoDTO>(await _repository.ObterComVariacoesAsync(id));

        public async Task<ProdutoDTO> Criar(ProdutoDTO dto)
        {
            var produto = _mapper.Map<Produto>(dto);
            produto.DataCriacao = DateTime.Now;
            produto.Validate();
            var criado = await _repository.CreateAsync(produto);
            return _mapper.Map<ProdutoDTO>(criado);
        }

        public async Task<ProdutoDTO> Atualizar(ProdutoDTO dto)
        {
            var existe = await _repository.ObterComVariacoesAsync(dto.Id);
            if (existe == null)
                throw new DomainException("Produto não encontrado.");

            var produto = _mapper.Map<Produto>(dto);
            produto.Validate();
            var atualizado = await _repository.AtualizarComVariacoesAsync(produto);
            return _mapper.Map<ProdutoDTO>(atualizado);
        }

        public async Task Excluir(long id) => await _repository.DeleteAsync(id);
    }
}
