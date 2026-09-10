using InstitutoTriboDeDavi.Application.Repositories;

namespace InstitutoTriboDeDavi.Application.Common
{
    // Miniatura de uma foto guardada no banco (FotoArquivo): devolve a guardada
    // ou gera na hora a partir da foto cheia e guarda para as próximas — assim
    // as fotos já existentes também ganham miniatura sem migração de dados.
    // Devolve null quando não dá (id não é do banco, imagem inválida) → o
    // chamador usa a foto cheia como fallback.
    public static class Miniaturas
    {
        public static async Task<byte[]> ObterOuGerarAsync(
            IFotoArquivoRepository repo, string fotoArquivoId, int maxLado)
        {
            if (!long.TryParse(fotoArquivoId, out var id)) return null;

            var arquivo = await repo.ObterAsync(id);
            if (arquivo?.Conteudo == null) return null;

            var mini = arquivo.Miniatura;
            if (mini == null || mini.Length == 0)
            {
                try { mini = Imagem.GerarMiniatura(arquivo.Conteudo, maxLado); }
                catch { return null; }
                await repo.SalvarMiniaturaAsync(id, mini);
            }
            return mini;
        }
    }
}
