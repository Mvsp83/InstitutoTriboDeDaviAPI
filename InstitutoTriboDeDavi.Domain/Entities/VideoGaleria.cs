using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Vídeo do YouTube exibido na galeria pública do site. Postado só por admin
    // (canal do instituto). Guardamos o id do vídeo (para embed/thumbnail) e a
    // URL original informada.
    public class VideoGaleria : Base
    {
        public string Titulo { get; set; } = string.Empty;
        public string YoutubeId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public System.DateTime CriadoEm { get; set; }

        public override bool Validate()
        {
            _errors.Clear();

            if (string.IsNullOrWhiteSpace(Titulo))
                _errors.Add("O título do vídeo é obrigatório.");
            else if (Titulo.Length > 150)
                _errors.Add("O título deve ter no máximo 150 caracteres.");

            if (string.IsNullOrWhiteSpace(YoutubeId))
                _errors.Add("Não foi possível identificar o vídeo do YouTube pela URL.");
            else if (YoutubeId.Length > 20)
                _errors.Add("Identificador de vídeo inválido.");

            if (Descricao != null && Descricao.Length > 500)
                _errors.Add("A descrição deve ter no máximo 500 caracteres.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
