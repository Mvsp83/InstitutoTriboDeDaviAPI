using System;

namespace InstitutoTriboDeDavi.API.ViewModels.Create
{
    // Busca pública para rematrícula: CPF do responsável + data de nascimento do
    // aluno (2 fatores, como o portal). Sem os dois, não devolve nada.
    public class BuscarRematriculaViewModel
    {
        public string CpfResponsavel { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
    }
}
