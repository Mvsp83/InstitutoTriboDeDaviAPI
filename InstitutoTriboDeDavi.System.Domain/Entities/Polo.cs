using InstitutoTriboDeDavi.Common.BaseEntity.BaseEntity;
using InstitutoTriboDeDavi.System.Core.Exceptions;
using InstitutoTriboDeDavi.System.Domain.Validators;

namespace InstitutoTriboDeDavi.System.Domain.Entities
{
    public class Polo : Base
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public long EnderecoId { get; set; }
        public Endereco Endereco { get; set; }

        public override bool Validate()
        {
            var validator = new PoloValidator();
            var validation = validator.Validate(this);

            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                {
                    _errors.Add(error.ErrorMessage);
                    throw new DomainException("Alguns campos estão inválidos!", _errors);
                }
            }

            return true;
        }
    }
}
