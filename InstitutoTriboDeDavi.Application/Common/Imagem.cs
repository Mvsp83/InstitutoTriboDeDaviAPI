using InstitutoTriboDeDavi.Domain.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace InstitutoTriboDeDavi.Application.Common
{
    public static class Imagem
    {
        // Teto de dimensões contra "decompression bomb": um arquivo pequeno pode
        // declarar dezenas de milhares de pixels por lado e estourar a memória ao
        // ser decodificado. 12000 px por lado e 40 MP cobrem fotos legítimas
        // (câmeras de celular/DSLR) e barram os payloads maliciosos.
        public const int MaxLado = 12_000;
        public const long MaxPixels = 40_000_000;

        // Lê só o cabeçalho (Image.Identify não decodifica os pixels) e valida as
        // dimensões antes de qualquer decode. Lança DomainException se exceder o
        // teto ou se o conteúdo não for uma imagem reconhecível.
        public static void ValidarDimensoes(byte[] conteudo)
        {
            IImageInfo info;
            try
            {
                using var ms = new MemoryStream(conteudo);
                info = Image.Identify(ms);
            }
            catch
            {
                throw new DomainException("Arquivo de imagem inválido ou não suportado.");
            }

            if (info == null)
                throw new DomainException("Arquivo de imagem inválido ou não suportado.");

            if (info.Width > MaxLado || info.Height > MaxLado ||
                (long)info.Width * info.Height > MaxPixels)
                throw new DomainException(
                    $"Imagem muito grande ({info.Width}x{info.Height}). " +
                    $"Máximo de {MaxLado}px por lado e {MaxPixels / 1_000_000} megapixels.");
        }

        // Gera uma miniatura JPEG que cabe em (maxLado × maxLado), preservando a
        // proporção. Usada em avatares e grades: reduz muito o payload das fotos
        // em listas (uma foto de vários MB vira alguns KB).
        public static byte[] GerarMiniatura(byte[] original, int maxLado = 128, int qualidade = 72)
        {
            // Guarda de segurança antes de decodificar (anti bomba de descompressão).
            ValidarDimensoes(original);

            using var entrada = new MemoryStream(original);
            using var image = Image.Load(entrada);

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxLado, maxLado),
            }));

            using var saida = new MemoryStream();
            image.Save(saida, new JpegEncoder { Quality = qualidade });
            return saida.ToArray();
        }
    }
}
