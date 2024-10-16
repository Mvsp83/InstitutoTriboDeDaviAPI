using InstitutoTriboDeDavi.Common.BaseEntity.BaseEntity;
using InstitutoTriboDeDavi.System.Core.Exceptions;
using InstitutoTriboDeDavi.System.Domain.Enums;
using InstitutoTriboDeDavi.System.Domain.Validators;

namespace InstitutoTriboDeDavi.System.Domain.Entities
{
    public class Responsavel : Base
    {
        public string Nome { get; set; }
        public Parentesco Parentesco { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }

        public override bool Validate()
        {
            var validator = new ResponsavelValidator();
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
