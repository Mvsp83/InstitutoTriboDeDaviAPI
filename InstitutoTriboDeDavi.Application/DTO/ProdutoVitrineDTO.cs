using System.Collections.Generic;

namespace InstitutoTriboDeDavi.Application.DTO
{
    // Versão PÚBLICA do produto (endpoint anônimo da vitrine). Não expõe estoque
    // exato, "Ativo" nem "DataCriacao" — só o necessário para o mostruário.
    public class ProdutoVitrineDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public string FotoArquivoId { get; set; }
        public bool TemFoto { get; set; }
        public string FormasPagamento { get; set; }
        public string Informacoes { get; set; }
        public List<VariacaoVitrineDTO> Variacoes { get; set; } = new();
    }

    public class VariacaoVitrineDTO
    {
        public long Id { get; set; }
        public string Tamanho { get; set; }
        public string Cor { get; set; }
        // Só um sinal de disponibilidade — sem revelar a quantidade em estoque.
        public bool Disponivel { get; set; }
    }
}
