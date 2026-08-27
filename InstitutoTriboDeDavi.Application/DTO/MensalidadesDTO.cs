using System;

namespace InstitutoTriboDeDavi.Application.DTO
{
    public class PlanoMensalidadeDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public int[] OpcoesVencimento { get; set; } = Array.Empty<int>();
        public bool Ativo { get; set; }
        public string Descricao { get; set; }
    }

    public class MatriculaFinanceiraDTO
    {
        public long Id { get; set; }
        public long AlunoId { get; set; }
        public long PlanoId { get; set; }
        public int DiaVencimento { get; set; }
        public string Inicio { get; set; } // "yyyy-MM"
        public string Status { get; set; }
        public string DescontoTipo { get; set; }
        public decimal DescontoValor { get; set; }
        public string Observacao { get; set; }
    }

    public class CobrancaDTO
    {
        public long Id { get; set; }
        public long AlunoId { get; set; }
        public long? PlanoId { get; set; }
        public string Competencia { get; set; } // "yyyy-MM"
        public DateTime Vencimento { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; }
        public DateTime? PagamentoData { get; set; }
        public decimal? PagamentoValor { get; set; }
        public string PagamentoForma { get; set; }
        public long? ContaId { get; set; }
        public long? MovimentacaoId { get; set; }
        public string Observacao { get; set; }
    }

    // Corpo do POST de geração das cobranças de um mês.
    public class GerarCobrancasDTO
    {
        public string Competencia { get; set; } // "yyyy-MM"
    }

    public class ResultadoGeracaoDTO
    {
        public int Geradas { get; set; }
        public int Ignoradas { get; set; }
        public string Mensagem { get; set; }
    }

    // Corpo do POST de baixa: registra o pagamento e lança no livro-caixa.
    public class BaixaCobrancaDTO
    {
        public long Id { get; set; }
        public DateTime PagamentoData { get; set; }
        public decimal PagamentoValor { get; set; }
        public string PagamentoForma { get; set; }
        public long ContaId { get; set; }
    }
}
