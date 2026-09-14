using AutoMapper;
using InstitutoTriboDeDavi.Application.DTO;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Services
{
    public class MensagemContatoService : IMensagemContatoService
    {
        private readonly IMapper _mapper;
        private readonly IMensagemContatoRepository _repository;

        public MensagemContatoService(IMapper mapper, IMensagemContatoRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<MensagemContatoDTO> Enviar(EnviarContatoDTO dto)
        {
            // Honeypot: campo oculto preenchido = bot → descarta silenciosamente.
            if (!string.IsNullOrWhiteSpace(dto.Website))
                return null;

            var msg = new MensagemContato
            {
                Nome = (dto.Nome ?? string.Empty).Trim(),
                Email = (dto.Email ?? string.Empty).Trim(),
                Telefone = (dto.Telefone ?? string.Empty).Trim(),
                Assunto = (dto.Assunto ?? string.Empty).Trim(),
                Mensagem = (dto.Mensagem ?? string.Empty).Trim(),
                DataCriacao = DateTime.Now,
                Lida = false,
            };
            msg.Validate();

            var criada = await _repository.CreateAsync(msg);
            return _mapper.Map<MensagemContatoDTO>(criada);
        }

        public async Task<List<MensagemContatoDTO>> Listar()
        {
            var lista = await _repository.ObterTodasAsync();
            return _mapper.Map<List<MensagemContatoDTO>>(lista);
        }

        public async Task MarcarLida(long id)
        {
            var msg = await _repository.GetByIdAsync(id);
            if (msg == null || msg.Lida)
                return;

            msg.Lida = true;
            await _repository.UpdateAsync(msg);
        }

        public async Task Excluir(long id)
        {
            await _repository.DeleteAsync(id);
        }

        public Task<int> ContarNaoLidas() => _repository.ContarNaoLidasAsync();
    }
}
