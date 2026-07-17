using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Domain.Validators;

namespace InstitutoTriboDeDavi.Domain.Entities.Business
{
    public class PlanoDeAula : Base
    {
        public long PoloId { get; set; }
        public int Turma { get; set; }
        public string Titulo { get; set; }
        public string Objetivo { get; set; }
        public DateTime DataPrevista { get; set; }
        public int DuracaoTotalMinutos { get; set; }
        public StatusPlano Status { get; set; }
        public long? AulaId { get; set; }
        public List<BlocoDoPlano> Blocos { get; set; } = new();

        public override bool Validate()
        {
            var validator = new PlanoDeAulaValidator();
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
