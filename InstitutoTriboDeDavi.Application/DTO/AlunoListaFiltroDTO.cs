namespace InstitutoTriboDeDavi.Application.DTO
{
    // Parâmetros da listagem paginada de alunos (vêm da query string).
    public class AlunoListaFiltroDTO
    {
        public int Pagina { get; set; } = 1;
        public int Tamanho { get; set; } = 50;
        // Filtro por nome (contém, sem diferenciar maiúsculas/acentos no banco).
        public string Busca { get; set; }
        public long? PoloId { get; set; }
        public int? Turma { get; set; }
        // Ordenação: "nome" (padrão), "faixa" ou "polo".
        public string OrdenarPor { get; set; } = "nome";
        // "asc" (padrão) ou "desc".
        public string Direcao { get; set; } = "asc";
    }
}
