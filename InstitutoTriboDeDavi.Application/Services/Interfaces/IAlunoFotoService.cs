using System.IO;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IAlunoFotoService
    {
        Task SalvarFoto(long alunoId, string nomeArquivo, string contentType, Stream conteudo);
        Task RemoverFoto(long alunoId);
        // Imagem em data URI (base64). null se não há foto / aluno não existe.
        // mini = true devolve a miniatura (avatar/grade), muito menor.
        Task<string> ObterFotoDataUri(long alunoId, bool mini = false);
        // Polo do aluno (para o controle de acesso). null se não existe.
        Task<long?> ObterPoloId(long alunoId);

        Task<ConfiguracaoFotoAlunoDTO> ObterConfig();
        Task SalvarConfig(ConfiguracaoFotoAlunoDTO dto);
    }
}
