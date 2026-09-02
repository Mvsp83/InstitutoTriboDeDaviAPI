using System.Collections.Generic;

namespace InstitutoTriboDeDavi.Application.DTO
{
    public class MembroGovernancaDTO
    {
        public long Id { get; set; }
        public int Ano { get; set; }
        public int Orgao { get; set; }
        public string Cargo { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public int Ordem { get; set; }
    }

    // Resposta pública da governança: os membros de um ano + os anos disponíveis
    // (para o seletor da página de Transparência).
    public class GovernancaPublicaDTO
    {
        public int Ano { get; set; }
        public List<int> Anos { get; set; } = new();
        public List<MembroGovernancaDTO> Membros { get; set; } = new();
    }
}
