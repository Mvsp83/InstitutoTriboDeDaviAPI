using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Evento do calendário oficial do instituto: datas de aulas (início,
    // término, graduação), datas comemorativas e atos administrativos (DRE,
    // balanço, fechamento financeiro, assembleias). PoloId nulo = vale para
    // todos os polos.
    public class EventoCalendario : Base
    {
        public int Ano { get; set; }
        public DateTime Data { get; set; }
        public DateTime? DataFim { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int Tipo { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public long? PoloId { get; set; }

        // Notificação por email. EmailsNotificacao aceita vários endereços
        // separados por vírgula ou ponto-e-vírgula. DiasAntecedencia = quantos
        // dias antes da data enviar (0 = no próprio dia). NotificacaoEnviada é
        // controlado pelo servidor (evita reenvio) e não vem do cliente.
        public bool Notificar { get; set; }
        public string EmailsNotificacao { get; set; } = string.Empty;
        public int DiasAntecedencia { get; set; }
        public bool NotificacaoEnviada { get; set; }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Titulo))
                _errors.Add("O título do evento é obrigatório.");
            if (Ano <= 0)
                _errors.Add("O ano do evento é inválido.");
            if (DataFim.HasValue && DataFim.Value < Data)
                _errors.Add("A data de término não pode ser anterior à data inicial.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
