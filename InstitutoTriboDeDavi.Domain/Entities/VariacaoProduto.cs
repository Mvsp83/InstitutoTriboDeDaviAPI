using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Uma variação vendável de um produto: combinação de tamanho e cor com o
    // próprio estoque. Tamanho ou cor podem ficar vazios (produto sem aquela
    // dimensão). Quantidade 0 = esgotado.
    public class VariacaoProduto : Base
    {
        public long ProdutoId { get; set; }
        public string Tamanho { get; set; } = string.Empty;
        public string Cor { get; set; } = string.Empty;
        public int Quantidade { get; set; }

        // Sem regras próprias — o produto valida o conjunto.
        public override bool Validate() => true;
    }
}
