namespace InstitutoTriboDeDavi.Application.Common
{
    // Resultado paginado genérico para as listagens. Mantém os itens da página
    // atual + os metadados que o cliente precisa para montar a navegação.
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Itens { get; set; } = new List<T>();
        public int Pagina { get; set; }
        public int Tamanho { get; set; }
        public int Total { get; set; }
        public int TotalPaginas => Tamanho > 0 ? (int)Math.Ceiling(Total / (double)Tamanho) : 0;
    }
}
