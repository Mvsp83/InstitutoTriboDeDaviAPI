using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Denúncia de um recado do mural: qualquer pessoa logada pode sinalizar um
    // anúncio impróprio; a equipe vê a fila (pendentes) e remove o recado ou
    // ignora a denúncia (marca como resolvida).
    public class DenunciaRecado : Base
    {
        public long RecadoId { get; set; }
        public string Motivo { get; set; } = string.Empty;
        // Quem denunciou: login da equipe ou "responsavel:{alunoId}" do portal.
        public string DenunciadoPor { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public bool Resolvida { get; set; }
        public string ResolvidoPor { get; set; } = string.Empty;
        public DateTime? DataResolucao { get; set; }

        public override bool Validate()
        {
            if (RecadoId <= 0)
                _errors.Add("O recado da denúncia é obrigatório.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
