using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities.Business
{
    // Registro do professor sobre um aluno, visível à família no portal:
    // - Advertência (Tipo=0): marca negativa de comportamento, com o motivo.
    // - Recado (Tipo=1): um status (da lista) + um texto livre opcional.
    public class OcorrenciaAluno : Base
    {
        public long AlunoId { get; set; }
        public long PoloId { get; set; }
        // 0 = Advertência, 1 = Recado.
        public int Tipo { get; set; }
        // Só no recado: índice do status escolhido (o front mapeia o rótulo).
        public int Status { get; set; }
        // Advertência: motivo. Recado: texto livre (pode ser vazio).
        public string Texto { get; set; } = string.Empty;
        public System.DateTime Data { get; set; }
        public string RegistradoPor { get; set; } = string.Empty;

        public override bool Validate() => true;
    }
}
