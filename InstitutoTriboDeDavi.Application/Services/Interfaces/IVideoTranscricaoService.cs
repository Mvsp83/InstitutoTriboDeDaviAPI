using System.Threading.Tasks;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    // Resultado da transcrição: o texto (vazio se não houver) e um aviso quando
    // não deu para trazer (sem legenda, erro de rede, etc.).
    public record TranscricaoResultado(string Texto, string Aviso);

    public interface IVideoTranscricaoService
    {
        // Busca a legenda do vídeo do YouTube JÁ TRADUZIDA para português (usa a
        // tradução automática do próprio YouTube). videoId = os 11 caracteres.
        Task<TranscricaoResultado> ObterTraduzidaAsync(string videoId);
    }
}
