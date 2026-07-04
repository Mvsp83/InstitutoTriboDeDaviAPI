using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.Infrastructure.Import
{
    public static class FaixaMapper
    {
        public static Faixa Mapear(string faixaTexto, string grausTexto)
        {
            var faixa = NormalizarFaixa(faixaTexto?.Trim().ToLower());
            var grau = NormalizarGrau(grausTexto?.Trim().ToLower());

            return faixa switch
            {
                "branca" or "iniciante" => grau switch
                {
                    1 => Faixa.Branca1,
                    2 => Faixa.Branca2,
                    3 => Faixa.Branca3,
                    4 => Faixa.Branca4,
                    _ => Faixa.Branca
                },
                "cinza" => grau switch
                {
                    1 => Faixa.Cinza1,
                    2 => Faixa.Cinza2,
                    3 => Faixa.Cinza3,
                    4 => Faixa.Cinza4,
                    _ => Faixa.Cinza
                },
                "amarela" => grau switch
                {
                    1 => Faixa.Amarela1,
                    2 => Faixa.Amarela2,
                    3 => Faixa.Amarela3,
                    4 => Faixa.Amarela4,
                    _ => Faixa.Amarela
                },
                "laranja" => grau switch
                {
                    1 => Faixa.Laranja1,
                    2 => Faixa.Laranja2,
                    3 => Faixa.Laranja3,
                    4 => Faixa.Laranja4,
                    _ => Faixa.Laranja
                },
                "verde" => grau switch
                {
                    1 => Faixa.Verde1,
                    2 => Faixa.Verde2,
                    3 => Faixa.Verde3,
                    4 => Faixa.Verde4,
                    _ => Faixa.Verde
                },
                "azul" => grau switch
                {
                    1 => Faixa.Azul1,
                    2 => Faixa.Azul2,
                    3 => Faixa.Azul3,
                    4 => Faixa.Azul4,
                    _ => Faixa.Azul
                },
                "roxa" => grau switch
                {
                    1 => Faixa.Roxa1,
                    2 => Faixa.Roxa2,
                    3 => Faixa.Roxa3,
                    4 => Faixa.Roxa4,
                    _ => Faixa.Roxa
                },
                "marrom" => grau switch
                {
                    1 => Faixa.Marrom1,
                    2 => Faixa.Marrom2,
                    3 => Faixa.Marrom3,
                    4 => Faixa.Marrom4,
                    _ => Faixa.Marrom
                },
                "preta" => Faixa.Preta,
                _ => Faixa.Branca
            };
        }

        private static string NormalizarFaixa(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "branca";

            // Remove acentos comuns
            var normalizado = texto
                .Replace("ã", "a")
                .Replace("á", "a")
                .Replace("â", "a")
                .Replace("é", "e")
                .Replace("ê", "e")
                .Replace("ó", "o")
                .Replace("ô", "o")
                .Trim();

            // Tolera o prefixo "Faixa" na resposta do formulário:
            // "faixa azul" → "azul" (sem isso, cairia no default Branca)
            if (normalizado.StartsWith("faixa "))
                normalizado = normalizado.Substring(6).Trim();

            // Primeira palavra: faixas compostas viram a cor base
            // ("cinza e branca" → "cinza")
            return normalizado.Split(' ')[0];
        }

        private static int NormalizarGrau(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return 0;
            if (int.TryParse(texto.Replace(".0", ""), out var grau)) return grau;
            return 0;
        }
    }
}
