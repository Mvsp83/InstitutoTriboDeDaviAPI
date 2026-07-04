using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Validators;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    public class Usuario : Base
    {
        public string Email { get; set; }
        public string Login { get; set; }
        public string SenhaHash { get; set; }
        public UserRole Role { get; set; }
        public long? PoloId { get; set; }
        public string PoloNome { get; set; }

        public override bool Validate()
        {
            var validator = new UsuarioValidator();
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
