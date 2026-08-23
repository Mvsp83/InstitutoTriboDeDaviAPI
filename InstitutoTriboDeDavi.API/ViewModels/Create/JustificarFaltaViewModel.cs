namespace InstitutoTriboDeDavi.API.ViewModels.Create
{
    // Justificativa de uma falta enviada pelo responsável no portal. A presença
    // é identificada pelo Id; o aluno vem do token, não do corpo.
    public class JustificarFaltaViewModel
    {
        public long PresencaId { get; set; }
        public string Justificativa { get; set; }
    }
}
