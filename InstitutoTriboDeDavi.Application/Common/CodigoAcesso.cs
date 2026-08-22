using System.Security.Cryptography;

namespace InstitutoTriboDeDavi.Application.Common
{
    // Gera o código de acesso do responsável ao portal. Compartilhado entre a
    // inscrição (gerado quando a família envia a ficha) e o cadastro do aluno
    // (o professor regenera se a família perder).
    public static class CodigoAcesso
    {
        // Alfabeto sem caracteres ambíguos (sem 0/O, 1/I/L) — o código é ditado
        // e digitado pela família, então precisa ser legível.
        private const string Alfabeto = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";

        public static string Gerar(int tamanho = 8)
        {
            var bytes = RandomNumberGenerator.GetBytes(tamanho);
            var chars = new char[tamanho];
            for (var i = 0; i < tamanho; i++)
                chars[i] = Alfabeto[bytes[i] % Alfabeto.Length];
            return new string(chars);
        }
    }
}
