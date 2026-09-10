using System.ComponentModel.DataAnnotations;

namespace InstitutoTriboDeDavi.Application.DTO
{
    public class PoloDTO
    {
        //[Required(ErrorMessage = "O Id não pode ser vazio")]
        //[Range(1, long.MaxValue, ErrorMessage = "O Id não pode ser menor que 1")]
        public long Id { get; set; }

        [Required(ErrorMessage = "O Nome não pode ser vazio.")]
        [MinLength(3, ErrorMessage = "O Nome deve ter no mínimo 3 caracteres.")]
        [MaxLength(20, ErrorMessage = "O Nome deve ter no máximo 20 caracteres.")]
        public string Nome { get; set; }
        public string Informacoes { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        // Limite de alunos ativos no ano (0 = sem limite).
        public int LimiteAlunos { get; set; }
        // Preenchido nas listagens: matrículas ativas do ano corrente neste polo.
        public int AlunosAtivos { get; set; }
        // Inscrições pendentes do ano neste polo (vaga reservada, ainda não
        // aprovada). Ativas + pendentes = ocupação para efeito de lotação.
        public int InscricoesPendentes { get; set; }
        public List<HorarioTurmaDTO> Horarios { get; set; } = new();
    }

    // Polo para exibição pública (página de Informações): dados do cadastro,
    // sem nada sensível.
    public class PoloPublicoDetalhadoDTO
    {
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Informacoes { get; set; }
        public List<HorarioTurmaDTO> Horarios { get; set; } = new();
        // Professores do polo que optaram por aparecer no site (foto + faixa).
        public List<ProfessorPublicoDTO> Professores { get; set; } = new();
    }

    // Professor exibido na seção pública do polo: só nome, faixa e foto de rosto
    // (a moldura recebe a cor da faixa no front). Nada sensível.
    public class ProfessorPublicoDTO
    {
        public string Nome { get; set; }
        public int? Faixa { get; set; }
        public string Foto { get; set; }
    }

    public class HorarioTurmaDTO
    {
        public long Id { get; set; }
        public long PoloId { get; set; }
        public int Turma { get; set; }
        public int DiaSemana { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
    }
}
