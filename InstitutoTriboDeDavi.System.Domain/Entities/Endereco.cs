using InstitutoTriboDeDavi.Common.BaseEntity.BaseEntity;
using InstitutoTriboDeDavi.System.Core.Exceptions;
using InstitutoTriboDeDavi.System.Domain.Validators;

namespace InstitutoTriboDeDavi.System.Domain.Entities
{
    public class Endereco : Base
    {
        public string Logradouro { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }
        public long BairroId { get; set; }
        public Bairro Bairro { get; set; }
        public long CidadeId { get; set; }
        public Cidade Cidade { get; set; }
        public long EstadoId { get; set; }
        public Estado Estado { get; set; }
        public long PaisId { get; set; }
        public Pais Pais { get; set; }

        public override bool Validate()
        {
            var validator = new EnderecoValidator();
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
