using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Domain.Validators;

namespace InstitutoTriboDeDavi.Domain.Entities.Business
{
    // Configuração de relatório salva pelo usuário no portal
    // (fonte de dados, colunas escolhidas e filtros fixos)
    public class RelatorioSalvo : Base
    {
        public string UsuarioLogin { get; set; }
        public string Nome { get; set; }
        public string FonteId { get; set; }
        public string Colunas { get; set; }
        public int? Turma { get; set; }
        public long? PoloId { get; set; }

        public override bool Validate()
        {
            var validator = new RelatorioSalvoValidator();
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
