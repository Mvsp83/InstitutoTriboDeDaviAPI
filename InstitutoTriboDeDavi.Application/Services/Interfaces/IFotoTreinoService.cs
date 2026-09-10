using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IFotoTreinoService
    {
        // Posta uma foto. Categoria "polo" respeita a trava 1/turma/aula e a
        // moderação por polo; as coleções do admin (graduacoes/geral/eventos)
        // entram publicadas e sem turma/polo.
        Task<FotoTreinoDTO> Postar(
            string categoria, long poloId, int turma, DateTime dataAula, string legenda,
            long professorId, string nomeArquivo, string contentType, Stream conteudo,
            bool consentimento);

        Task<List<FotoTreinoDTO>> Listar();                // admin/moderação
        Task<List<FotoTreinoPublicaDTO>> ListarPublicas(); // álbum público

        Task DefinirPublicacao(long id, bool publicada);
        Task Excluir(long id);

        // Metadados de uma foto (o controller usa p/ decidir acesso ao arquivo).
        Task<FotoTreinoDTO> Obter(long id);
        // Stream do binário a partir do storage.
        Task<FotoDownload> BaixarArquivo(long id);
        // Miniatura (grade da galeria), gerada sob demanda.
        Task<FotoDownload> BaixarMiniatura(long id);
        // Imagem em data URI (base64) para preview de moderação. null se não existe.
        Task<string> ObterPreviaDataUri(long id);

        // Config por polo do fluxo de publicação.
        Task<List<PoloFotoConfigDTO>> ListarConfigPolos();
        Task DefinirConfigPolo(long poloId, bool requerAutorizacao);
    }
}
