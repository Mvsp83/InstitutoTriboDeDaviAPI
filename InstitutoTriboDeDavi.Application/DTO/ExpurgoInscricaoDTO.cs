using System;

namespace InstitutoTriboDeDavi.Application.DTO
{
    // Inscrição recusada e antiga, candidata ao expurgo de dados pessoais
    // (retenção/LGPD). Traz só o mínimo para o admin reconhecer e decidir.
    public class ExpurgoInscricaoDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Ano { get; set; }
        public DateTime DataEnvio { get; set; }
    }
}
