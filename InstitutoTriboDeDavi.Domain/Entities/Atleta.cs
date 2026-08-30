using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Atleta de alto rendimento: um aluno do cadastro selecionado para
    // acompanhamento (índices físicos, competições, diário, metas). O aluno
    // continua no cadastro normal; este registro guarda os dados esportivos.
    public class Atleta : Base
    {
        public long AlunoId { get; set; }
        public string CategoriaPeso { get; set; } = string.Empty;
        public string Objetivo { get; set; } = string.Empty;
        public int Status { get; set; } // StatusAtleta
        public DateTime DataInclusao { get; set; }
        public bool Ativo { get; set; } = true;

        public List<AvaliacaoFisica> Avaliacoes { get; set; } = new();
        public List<Competicao> Competicoes { get; set; } = new();
        public List<AnotacaoAtleta> Anotacoes { get; set; } = new();
        public List<MetaAtleta> Metas { get; set; } = new();

        public override bool Validate()
        {
            if (AlunoId <= 0)
                _errors.Add("O atleta precisa estar vinculado a um aluno.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
