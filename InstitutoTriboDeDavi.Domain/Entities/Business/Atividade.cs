using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Domain.Validators;

namespace InstitutoTriboDeDavi.Domain.Entities.Business
{
    public class Atividade : Base
    {
        public string Nome { get; set; }
        public TipoBloco Tipo { get; set; }
        public string Descricao { get; set; }
        public string Tags { get; set; }
        public string Principio { get; set; }
        public string ReferenciaBiblica { get; set; }
        public string VideoUrl { get; set; }

        public override bool Validate()
        {
            var validator = new AtividadeValidator();
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
