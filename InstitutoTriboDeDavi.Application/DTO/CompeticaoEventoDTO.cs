namespace InstitutoTriboDeDavi.Application.DTO
{
    public class CompeticaoEventoDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public DateTime Data { get; set; }
        public DateTime? DataFim { get; set; }
        public string Local { get; set; }
        public string Organizador { get; set; }
        public DateTime? PrazoInscricao { get; set; }
        public string Link { get; set; }
        public string Observacao { get; set; }
        public int Status { get; set; }

        // Preenchido na listagem: quantos atletas participam.
        public int TotalParticipantes { get; set; }

        public List<ParticipacaoAtletaDTO> Participacoes { get; set; } = new();
    }

    public class ParticipacaoAtletaDTO
    {
        public long Id { get; set; }
        public long CompeticaoEventoId { get; set; }
        public long AtletaId { get; set; }
        public string CategoriaPeso { get; set; }
        public int Colocacao { get; set; }
        public int Lutas { get; set; }
        public int Vitorias { get; set; }
        public int Finalizacoes { get; set; }
        public string Observacao { get; set; }
        // Preenchido para exibição.
        public string AtletaNome { get; set; }
        public int Faixa { get; set; }
    }
}
