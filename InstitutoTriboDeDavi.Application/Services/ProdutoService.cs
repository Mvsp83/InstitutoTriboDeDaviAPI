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

        public async Task<List<ProdutoDTO>> Vitrine() =>
            _mapper.Map<List<ProdutoDTO>>(await _repository.ListarVitrineAsync());

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
