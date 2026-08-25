using System;

namespace InstitutoTriboDeDavi.Application.DTO
{
    // Números crus por aluno para o front decidir a aptidão ao exame, aplicando
    // os parâmetros por faixa configurados na tela de Parâmetros de Graduação.
    // "DesdeUltima" = a partir da última graduação (ou de sempre, se nunca
    // graduou — DataUltimaGraduacao nula).
    public class AptidaoGraduacaoDTO
    {
        public long AlunoId { get; set; }
        public int Faixa { get; set; }
        public long PoloId { get; set; }
        public DateTime? DataUltimaGraduacao { get; set; }
        public int PresencasDesdeUltima { get; set; }
        public int AdvertenciasDesdeUltima { get; set; }
    }
}
