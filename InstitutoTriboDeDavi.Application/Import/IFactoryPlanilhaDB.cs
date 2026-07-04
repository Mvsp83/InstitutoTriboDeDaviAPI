namespace InstitutoTriboDeDavi.Application.Import
{
    public interface IFactoryPlanilhaDB
    {
        Task<ImportacaoResultado> ImportarAlunosAsync(Stream arquivo, long poloIdPadrao);
        Task<ImportacaoResultado> ImportarAlunosDeSheetsAsync(IList<IList<object>> rows, long poloId);

    }

    public class ImportacaoResultado
    {
        public string PoloNome { get; set; }
        public int Inseridos { get; set; }
        public int Atualizados { get; set; }
        public int Ignorados { get; set; }
        public List<string> Erros { get; set; } = new();
    }
}
