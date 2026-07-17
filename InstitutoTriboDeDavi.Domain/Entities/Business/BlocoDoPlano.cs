using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Domain.Validators;

namespace InstitutoTriboDeDavi.Domain.Entities.Business
{
    public class BlocoDoPlano : Base
    {
        public long PlanoDeAulaId { get; set; }
        public int Ordem { get; set; }
        public string Nome { get; set; }
        public TipoBloco Tipo { get; set; }
        public int DuracaoMinutos { get; set; }
        public string Descricao { get; set; }
        public List<AtividadeDoBloco> Atividades { get; set; } = new();

        public override bool Validate()
        {
            var validator = new BlocoDoPlanoValidator();
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
