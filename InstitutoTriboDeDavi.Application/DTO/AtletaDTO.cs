namespace InstitutoTriboDeDavi.Application.DTO
{
    public class AtletaDTO
    {
        public long Id { get; set; }
        public long AlunoId { get; set; }
        public string CategoriaPeso { get; set; }
        public string Objetivo { get; set; }
        public int Status { get; set; }
        public DateTime DataInclusao { get; set; }
        public bool Ativo { get; set; }

        // Preenchidos a partir do cadastro do aluno (exibição).
        public string AlunoNome { get; set; }
        public int Faixa { get; set; }
        public string PoloNome { get; set; }

        // Carregados no detalhe.
        public List<AvaliacaoFisicaDTO> Avaliacoes { get; set; } = new();
        public List<CompeticaoDTO> Competicoes { get; set; } = new();
        public List<AnotacaoAtletaDTO> Anotacoes { get; set; } = new();
        public List<MetaAtletaDTO> Metas { get; set; } = new();
    }

    public class AvaliacaoFisicaDTO
    {
        public long Id { get; set; }
        public long AtletaId { get; set; }
        public DateTime Data { get; set; }
        public string Observacao { get; set; }
        public List<IndicadorAvaliacaoDTO> Indicadores { get; set; } = new();
    }

    public class IndicadorAvaliacaoDTO
    {
        public long Id { get; set; }
        public long AvaliacaoFisicaId { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public string Unidade { get; set; }
    }

    public class CompeticaoDTO
    {
        public long Id { get; set; }
        public long AtletaId { get; set; }
        public DateTime Data { get; set; }
        public string Evento { get; set; }
        public string CategoriaPeso { get; set; }
        public int Colocacao { get; set; }
        public int Lutas { get; set; }
        public int Vitorias { get; set; }
        public int Finalizacoes { get; set; }
        public string Observacao { get; set; }
    }

    public class AnotacaoAtletaDTO
    {
        public long Id { get; set; }
        public long AtletaId { get; set; }
        public DateTime Data { get; set; }
        public string Texto { get; set; }
        public string Autor { get; set; }
    }

    public class MetaAtletaDTO
    {
        public long Id { get; set; }
        public long AtletaId { get; set; }
        public string Descricao { get; set; }
        public DateTime? Prazo { get; set; }
        public int Status { get; set; }
        public DateTime? DataConclusao { get; set; }
    }
}
