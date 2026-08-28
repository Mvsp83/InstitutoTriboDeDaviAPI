namespace InstitutoTriboDeDavi.Application.DTO
{
    // Config global de onde a foto do aluno aparece.
    public class ConfiguracaoFotoAlunoDTO
    {
        public bool MostrarNoCadastro { get; set; }
        public bool MostrarNaChamada { get; set; }
        public bool MostrarNoResponsavel { get; set; }
        public bool MostrarNaCarteirinha { get; set; }
    }
}
