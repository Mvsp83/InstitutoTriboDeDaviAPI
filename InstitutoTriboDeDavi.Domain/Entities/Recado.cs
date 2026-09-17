using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Mural de recados: classificados da comunidade (emprego, venda de veículo,
    // serviços, achados e perdidos...) que um polo divulga para todos os demais.
    // No MVP quem publica é a equipe (em nome do aluno/responsável); fica visível
    // a quem está logado (portal do aluno + painel). Expira sozinho por ExpiraEm.
    public class Recado : Base
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        // 0=Emprego, 1=Veículo, 2=Imóvel, 3=Serviços, 4=Achados e Perdidos, 5=Outros
        public int Categoria { get; set; }
        // Nome de quem anuncia (opcional) e como falar com a pessoa (obrigatório).
        public string Anunciante { get; set; } = string.Empty;
        public string Contato { get; set; } = string.Empty;
        // Polo de origem (do autor). Nulo = geral/administração.
        public long? PoloId { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime ExpiraEm { get; set; }
        public string CriadoPor { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Titulo))
                _errors.Add("O título do recado é obrigatório.");
            if (string.IsNullOrWhiteSpace(Descricao))
                _errors.Add("A descrição do recado é obrigatória.");
            if (string.IsNullOrWhiteSpace(Contato))
                _errors.Add("Informe um contato para o recado.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
