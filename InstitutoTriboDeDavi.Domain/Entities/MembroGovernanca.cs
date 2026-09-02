using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    public enum OrgaoGovernanca
    {
        Diretoria = 0,
        ConselhoFiscal = 1,
    }

    // Membro da diretoria ou do conselho fiscal, num ano de vigência. A página
    // pública de Transparência lista a governança a partir daqui — não mais de
    // texto fixo no front. Cargo é livre (Presidente, Vice-Presidente, 1º
    // Secretário..., Titular, Suplente); Ordem controla a exibição.
    public class MembroGovernanca : Base
    {
        public int Ano { get; set; }
        public int Orgao { get; set; }
        public string Cargo { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public int Ordem { get; set; }

        public override bool Validate()
        {
            _errors.Clear();

            if (Ano < 2000)
                _errors.Add("Ano de vigência inválido.");
            if (string.IsNullOrWhiteSpace(Cargo))
                _errors.Add("O cargo é obrigatório.");
            if (string.IsNullOrWhiteSpace(Nome))
                _errors.Add("O nome é obrigatório.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
