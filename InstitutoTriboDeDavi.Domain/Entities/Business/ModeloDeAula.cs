using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Domain.Validators;

namespace InstitutoTriboDeDavi.Domain.Entities.Business
{
    public class ModeloDeAula : Base
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int DuracaoTotalMinutos { get; set; }
        public List<BlocoDoModelo> Blocos { get; set; } = new();

        public override bool Validate()
        {
            var validator = new ModeloDeAulaValidator();
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
