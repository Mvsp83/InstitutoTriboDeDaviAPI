using InstitutoTriboDeDavi.Common.BaseEntity.BaseEntity;
using InstitutoTriboDeDavi.System.Core.Exceptions;
using InstitutoTriboDeDavi.System.Domain.Validators;

namespace InstitutoTriboDeDavi.System.Domain.Entities
{
    public class Usuario : Base
    {
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        public override bool Validate()
        {
            var validator = new UsuarioValidator();
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
