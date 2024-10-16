using InstitutoTriboDeDavi.Common.BaseEntity.BaseEntity;
using InstitutoTriboDeDavi.System.Core.Exceptions;
using InstitutoTriboDeDavi.System.Domain.Validators;

namespace InstitutoTriboDeDavi.System.Domain.Entities
{
    public class Pais : Base
    {
        public string Nome { get; set; }
        public string Sigla { get; set; }

        public override bool Validate()
        {
            var validator = new PaisValidator();
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
