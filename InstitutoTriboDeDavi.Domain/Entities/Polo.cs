using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Domain.Validators;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    public class Polo : Base
    {
        public string Nome { get; set; }
        public string Informacoes { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }

        // Limite de alunos ativos no ano. 0 = sem limite. Ao atingir, novas
        // inscrições para este polo são bloqueadas até liberar vaga (inativar
        // matrícula ou aumentar o limite).
        public int LimiteAlunos { get; set; }

        // O polo tem turma de adultos? Quando false, a inscrição de adultos é
        // bloqueada neste polo (a ficha infantil segue permitida). Default true
        // para não mudar o comportamento dos polos já existentes.
        public bool AceitaAdultos { get; set; } = true;

        // Horários de treino por turma (um item por dia da semana).
        public List<HorarioTurma> Horarios { get; set; } = new();

        public override bool Validate()
        {
            var validator = new PoloValidator();
            var validation = validator.Validate(this);

            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                {
                    _errors.Add(error.ErrorMessage);
                }

                throw new DomainException("Alguns campos estão inválidos!", _errors);
            }

            return true;
        }
    }
}
