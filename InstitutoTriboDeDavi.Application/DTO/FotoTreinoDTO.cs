using System;

namespace InstitutoTriboDeDavi.Application.DTO
{
    // Visão administrativa/interna de uma foto de treino (moderação e gestão).
    public class FotoTreinoDTO
    {
        public long Id { get; set; }
        public string Categoria { get; set; }
        public long PoloId { get; set; }
        public string PoloNome { get; set; }
        public int Turma { get; set; }
        public DateTime DataAula { get; set; }
        public string Legenda { get; set; }
        public long ProfessorId { get; set; }
        public bool Publicada { get; set; }
        public DateTime CriadoEm { get; set; }
        // URL estável servida pela API (stream do binário).
        public string Url { get; set; }
    }

    // Visão pública (álbum do site) — só o necessário, sem ids internos sensíveis.
    public class FotoTreinoPublicaDTO
    {
        public long Id { get; set; }
        public string Categoria { get; set; }
        public string PoloNome { get; set; }
        public int Turma { get; set; }
        public DateTime DataAula { get; set; }
        public string Legenda { get; set; }
        public string Url { get; set; }
    }

    // Config por polo do fluxo de publicação das fotos.
    public class PoloFotoConfigDTO
    {
        public long PoloId { get; set; }
        public string PoloNome { get; set; }
        public bool RequerAutorizacao { get; set; }
    }
}
