using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace InstitutoTriboDeDavi.Application.Common
{
    public static class Imagem
    {
        // Gera uma miniatura JPEG que cabe em (maxLado × maxLado), preservando a
        // proporção. Usada em avatares e grades: reduz muito o payload das fotos
        // em listas (uma foto de vários MB vira alguns KB).
        public static byte[] GerarMiniatura(byte[] original, int maxLado = 128, int qualidade = 72)
        {
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
