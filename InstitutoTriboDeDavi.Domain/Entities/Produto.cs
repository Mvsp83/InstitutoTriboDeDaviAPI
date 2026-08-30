using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Produto da loja virtual (camisetas, hashguards, etc.). Cadastrado pelo
    // admin e exibido na vitrine pública quando Ativo. O estoque é por variação
    // (tamanho + cor); a foto fica no storage (IFotoStorage) via FotoArquivoId.
    public class Produto : Base
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string FotoArquivoId { get; set; } = string.Empty;
        // Texto livre com as formas de pagamento aceitas (ex.: "Pix, dinheiro").
        public string FormasPagamento { get; set; } = string.Empty;
        // Observações gerais (prazo, retirada, troca, etc.).
        public string Informacoes { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; }

        public List<VariacaoProduto> Variacoes { get; set; } = new();

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Nome))
                _errors.Add("O nome do produto é obrigatório.");
            if (Preco < 0)
                _errors.Add("O preço não pode ser negativo.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
