using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities.Business
{
    // Contador diário agregado de métricas do site (1 linha por dia + chave, não
    // por acesso). Ex.: ("2026-09-10", "pageview:/matricula", 34). Mantém o custo
    // desprezível: a tabela cresce por dia × chave, não por número de visitas.
    public class MetricaDiaria : Base
    {
        public DateTime Data { get; set; }   // dia (sem hora)
        public string Chave { get; set; }    // ex.: "pageview:/", "evento:doar_click"
        public long Valor { get; set; }

        public override bool Validate() => !string.IsNullOrWhiteSpace(Chave);
    }
}
