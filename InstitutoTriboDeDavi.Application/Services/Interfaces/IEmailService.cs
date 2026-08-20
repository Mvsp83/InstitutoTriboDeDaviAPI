namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IEmailService
    {
        // Envia um email HTML para um ou mais destinatários.
        Task EnviarAsync(IEnumerable<string> destinatarios, string assunto, string corpoHtml);
    }
}
