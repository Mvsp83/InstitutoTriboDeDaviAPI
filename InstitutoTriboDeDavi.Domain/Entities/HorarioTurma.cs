using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Horário de treino de uma turma de um polo. Uma turma pode ter vários
    // horários (um por dia da semana). DiaSemana segue o padrão .NET DayOfWeek
    // (0=Domingo ... 6=Sábado). Hora em texto "HH:mm".
    public class HorarioTurma : Base
    {
        public long PoloId { get; set; }
        public int Turma { get; set; }
        public int DiaSemana { get; set; }
        public string HoraInicio { get; set; } = string.Empty;
        public string HoraFim { get; set; } = string.Empty;

        public override bool Validate()
        {
            if (Turma <= 0)
                _errors.Add("A turma do horário é obrigatória.");
            if (string.IsNullOrWhiteSpace(HoraInicio))
                _errors.Add("Informe o horário de início.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
