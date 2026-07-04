using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Validators;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    public class Aluno : Base
    {
        public string Nome { get; set; }
        public string? RG { get; set; }
        public string? CPF { get; set; }
        public DateTime DataNascimento { get; set; }
        public double? Peso { get; set; }
        public Faixa Faixa { get; set; }
        public string? Endereco { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Celular { get; set; }
        public string? Responsavel { get; set; }
        public Parentesco? Parentesco { get; set; }
        public string? RGResponsavel { get; set; }
        public string? CPFResponsavel { get; set; }
        public string? Escola { get; set; }
        public string? Periodo { get; set; }
        public long PoloId { get; set; }
        public int Turma { get; set; }

        public override bool Validate()
        {
            var validator = new AlunoValidator();
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
