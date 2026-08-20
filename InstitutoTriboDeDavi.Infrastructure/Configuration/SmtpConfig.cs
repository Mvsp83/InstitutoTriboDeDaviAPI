namespace InstitutoTriboDeDavi.Infrastructure.Configuration
{
    // Envio de email (avisos do calendário). Segredos (User/Password) NÃO ficam
    // no appsettings — dev: user-secrets; produção: env Smtp__User / Smtp__Password.
    public class SmtpConfig
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        // Remetente exibido; se vazio, usa User.
        public string From { get; set; } = string.Empty;
        // Horário diário (HH:mm, horário de Brasília) em que o job de avisos roda.
        public string HorarioExecucao { get; set; } = "07:00";
    }
}
