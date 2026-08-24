using System;

namespace InstitutoTriboDeDavi.Application.DTO
{
    // Ocorrência (advertência ou recado) de um aluno, para a equipe registrar/ver.
    public class OcorrenciaAlunoDTO
    {
        public long Id { get; set; }
        public long AlunoId { get; set; }
        public int Tipo { get; set; }   // 0 = Advertência, 1 = Recado
        public int Status { get; set; } // só no recado
        public string Texto { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string RegistradoPor { get; set; } = string.Empty;
    }
}
