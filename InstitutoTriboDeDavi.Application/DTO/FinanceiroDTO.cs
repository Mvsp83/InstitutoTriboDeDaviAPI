using System;

namespace InstitutoTriboDeDavi.Application.DTO
{
    public class ContaFinanceiraDTO
    {
        public long Id { get; set; }
        public string Tipo { get; set; }
        public string Nome { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string Numero { get; set; }
        public decimal SaldoInicial { get; set; }
        public bool Ativa { get; set; }
        public string Observacoes { get; set; }
    }

    public class MovimentacaoFinanceiraDTO
    {
        public long Id { get; set; }
        public long ContaId { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public string CategoriaId { get; set; }
        public string Tipo { get; set; }
        public decimal Valor { get; set; }
        public bool Conciliado { get; set; }
        public string Documento { get; set; }
        public string Observacoes { get; set; }
        public string TransferenciaId { get; set; }
    }

    // Transferência entre contas: vira dois lançamentos ligados, gravados juntos.
    public class TransferenciaDTO
    {
        public long ContaOrigemId { get; set; }
        public long ContaDestinoId { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public string CategoriaId { get; set; }
        public string Descricao { get; set; }
        public string Documento { get; set; }
        public string Observacoes { get; set; }
    }

    // Carga inicial vinda do navegador (migração do localStorage).
    public class ImportacaoFinanceiraDTO
    {
        public ContaFinanceiraDTO[] Contas { get; set; }
        public MovimentacaoFinanceiraDTO[] Movimentacoes { get; set; }
    }

    public class ResultadoImportacaoDTO
    {
        public int ContasImportadas { get; set; }
        public int MovimentacoesImportadas { get; set; }
        public string Mensagem { get; set; }
    }
}
