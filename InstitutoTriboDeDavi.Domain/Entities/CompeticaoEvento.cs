using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Competição (evento/campeonato) que o instituto acompanha: dados do evento
    // e os atletas participantes com seus resultados.
    public class CompeticaoEvento : Base
    {
        public string Nome { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public DateTime? DataFim { get; set; }
        public string Local { get; set; } = string.Empty;
        public string Organizador { get; set; } = string.Empty;
        public DateTime? PrazoInscricao { get; set; }
        public string Link { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public int Status { get; set; } // StatusCompeticao

        public List<ParticipacaoAtleta> Participacoes { get; set; } = new();

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Nome))
                _errors.Add("O nome da competição é obrigatório.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }

    // Participação de um atleta numa competição, com o resultado.
    public class ParticipacaoAtleta : Base
    {
        public long CompeticaoEventoId { get; set; }
        public long AtletaId { get; set; }
        public string CategoriaPeso { get; set; } = string.Empty;
        public int Colocacao { get; set; } // 0 = sem pódio/não informado
        public int Lutas { get; set; }
        public int Vitorias { get; set; }
        public int Finalizacoes { get; set; }
        public string Observacao { get; set; } = string.Empty;

        public override bool Validate() => true;
    }
}
