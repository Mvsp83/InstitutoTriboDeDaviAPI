namespace InstitutoTriboDeDavi.Application.DTO
{
    // Entrada do beacon público: um evento a contabilizar.
    public class MetricaEventoDTO
    {
        // "pageview", "doar_click", "inscricao_ok", "responsavel_acesso",
        // "davizinho" — validado por lista branca no serviço.
        public string Evento { get; set; }
        // Complemento do evento: caminho da página (pageview) ou o texto da
        // pergunta (davizinho). Opcional.
        public string Dimensao { get; set; }
    }

    public class SerieDiaDTO
    {
        public DateTime Data { get; set; }
        public long Valor { get; set; }
    }

    public class ItemContagemDTO
    {
        public string Rotulo { get; set; }
        public long Valor { get; set; }
    }

    // Resumo agregado para a tela "Acessos ao site" (admin).
    public class MetricaResumoDTO
    {
        public int Dias { get; set; }
        public long Visitas { get; set; }
        public long DoarCliques { get; set; }
        public long InscricoesConcluidas { get; set; }
        public long AcessosResponsavel { get; set; }
        public List<SerieDiaDTO> VisitasPorDia { get; set; } = new();
        public List<ItemContagemDTO> TopPaginas { get; set; } = new();
        public List<ItemContagemDTO> TopDavizinho { get; set; } = new();
        // Fase 2: origem do tráfego, dispositivo e funil de inscrição.
        public List<ItemContagemDTO> Origem { get; set; } = new();
        public List<ItemContagemDTO> Dispositivos { get; set; } = new();
        public List<ItemContagemDTO> FunilInscricao { get; set; } = new();
    }
}
