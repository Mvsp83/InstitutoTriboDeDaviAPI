using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Bem do patrimônio do instituto (quimonos, faixas, tatames, veículos,
    // imóveis, equipamentos, etc.). PoloId nulo = bem geral / não vinculado.
    public class BemPatrimonial : Base
    {
        public int Categoria { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public DateTime? DataAquisicao { get; set; }
        public int Estado { get; set; }
        public long? PoloId { get; set; }
        public string NumeroPatrimonio { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Descricao))
                _errors.Add("A descrição do bem é obrigatória.");
            if (Quantidade < 0)
                _errors.Add("A quantidade não pode ser negativa.");
            if (ValorUnitario < 0)
                _errors.Add("O valor unitário não pode ser negativo.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
