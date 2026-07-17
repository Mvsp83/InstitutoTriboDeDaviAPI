using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.Domain.Entities.Consultas
{
    // Resultado da consulta de histórico: quando cada atividade
    // foi trabalhada pela última vez em uma turma
    public class HistoricoAtividade
    {
        public long AtividadeId { get; set; }
        public string Nome { get; set; }
        public TipoBloco Tipo { get; set; }
        public DateTime UltimaData { get; set; }
        public int Vezes { get; set; }
    }
}
