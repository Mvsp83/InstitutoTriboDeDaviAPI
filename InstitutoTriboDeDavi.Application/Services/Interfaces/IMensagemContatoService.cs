using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IMensagemContatoService
    {
        // Registra a mensagem do Fale Conosco. Devolve null quando é descartada
        // pelo honeypot (bot) — o controller responde sucesso mesmo assim.
        Task<MensagemContatoDTO> Enviar(EnviarContatoDTO dto);
        Task<List<MensagemContatoDTO>> Listar();
        Task MarcarLida(long id);
        Task Excluir(long id);
        Task<int> ContarNaoLidas();
    }
}
