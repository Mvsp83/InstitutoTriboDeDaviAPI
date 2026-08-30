namespace InstitutoTriboDeDavi.Application.DTO
{
    public class ProdutoDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public string FotoArquivoId { get; set; }
        // Conveniência para o front: há foto para exibir na vitrine?
        public bool TemFoto { get; set; }
        public string FormasPagamento { get; set; }
        public string Informacoes { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public List<VariacaoProdutoDTO> Variacoes { get; set; } = new();
    }

    public class VariacaoProdutoDTO
    {
        public long Id { get; set; }
        public long ProdutoId { get; set; }
        public string Tamanho { get; set; }
        public string Cor { get; set; }
        public int Quantidade { get; set; }
    }
}
