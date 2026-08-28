using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Config global (linha única) de onde a foto do aluno aparece. O admin
    // liga/desliga por tela; vale para todos os usuários.
    public class ConfiguracaoFotoAluno : Base
    {
        public bool MostrarNoCadastro { get; set; } = true;
        public bool MostrarNaChamada { get; set; } = true;
        public bool MostrarNoResponsavel { get; set; } = true;
        public bool MostrarNaCarteirinha { get; set; } = true;

        public override bool Validate() => true;
    }
}
