namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    // Resumo do processamento dos avisos do calendário (job diário ou disparo
    // manual). Erros traz o motivo por evento que falhou (ex.: SMTP não
    // configurado), útil para exibir na tela ao testar.
    public class NotificacaoCalendarioResultado
    {
        public int Processados { get; set; }
        public int Enviados { get; set; }
        public int Ignorados { get; set; }
        public List<string> Erros { get; set; } = new();
    }

    public interface INotificacaoCalendarioService
    {
        // Processa os eventos com notificação pendente. forcarEnvio = true envia
        // todos, ignorando a janela de data (usado no disparo manual/teste);
        // false aplica a regra da data de disparo (usado pelo job diário).
        Task<NotificacaoCalendarioResultado> ProcessarAsync(bool forcarEnvio = false);
    }
}
