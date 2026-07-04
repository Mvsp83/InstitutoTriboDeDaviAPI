using System.ComponentModel.DataAnnotations;

namespace InstitutoTriboDeDavi.Application.DTO
{
    public class AtribuirTurmaDTO
    {
        [Required(ErrorMessage = "O Id do aluno não pode ser vazio.")]
        [Range(1, long.MaxValue, ErrorMessage = "O Id do aluno deve ser maior que zero.")]
        public long AlunoId { get; set; }

        [Required(ErrorMessage = "A Turma não pode ser vazia.")]
        [Range(1, 99, ErrorMessage = "A Turma deve ser entre 1 e 99.")]
        public int Turma { get; set; }
    }
}
