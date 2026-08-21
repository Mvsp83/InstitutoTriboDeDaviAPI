using System;

namespace InstitutoTriboDeDavi.Application.DTO
{
    public class DoadorDTO
    {
        public long Id { get; set; }
        public int TipoPessoa { get; set; }
        public string Nome { get; set; }
        public string Documento { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string Observacoes { get; set; }
        public bool Ativo { get; set; }
        // Resumo calculado, para a lista mostrar quem sustenta o projeto.
        public decimal TotalDoado { get; set; }
        public int QuantidadeDoacoes { get; set; }
        public DateTime? UltimaDoacao { get; set; }
    }

    public class DoacaoDTO
    {
        public long Id { get; set; }
        public long? DoadorId { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public string Forma { get; set; }
        public string Finalidade { get; set; }
        public string Observacoes { get; set; }
        public long? ReciboDocumentoId { get; set; }
        public string ReciboNumero { get; set; }
        public string RegistradoPor { get; set; }
        // Preenchido na listagem.
        public string NomeDoador { get; set; }
    }

    // Totais do período, para o painel de captação.
    public class ResumoDoacoesDTO
    {
        public int Ano { get; set; }
        public decimal Total { get; set; }
        public int Quantidade { get; set; }
        public int Doadores { get; set; }
        public decimal TicketMedio { get; set; }
    }
}
