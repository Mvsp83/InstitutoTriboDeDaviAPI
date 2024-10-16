using InstitutoTriboDeDavi.Common.BaseEntity.BaseEntity;
using InstitutoTriboDeDavi.System.Core.Exceptions;
using InstitutoTriboDeDavi.System.Domain.Enum;
using InstitutoTriboDeDavi.System.Domain.Validators;

namespace InstitutoTriboDeDavi.System.Domain.Entities
{
    public class Aluno : Base
    {
        public string Nome { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        public DateTime DataNascimento { get; set; }
        public double Peso { get; set; }
        public Faixa Faixa { get; set; }
        public long EnderecoId { get; set; }
        public Endereco Endereco { get; set; }
        public long ResponsavelId { get; set; }
        public Responsavel Responsavel { get; set; }
        public string Celular { get; set; }

        public override bool Validate()
        {
            var validator = new AlunoValidator();
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
