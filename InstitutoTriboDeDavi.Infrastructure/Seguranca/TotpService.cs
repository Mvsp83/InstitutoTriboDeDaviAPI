using System;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using OtpNet;

namespace InstitutoTriboDeDavi.Infrastructure.Seguranca
{
    public class TotpService : ITotpService
    {
        public string GerarSecret()
        {
            // 20 bytes = 160 bits, o tamanho recomendado para HMAC-SHA1/TOTP.
            var key = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(key);
        }

        public string GerarUri(string secret, string conta, string emissor)
        {
            var e = Uri.EscapeDataString(emissor);
            var c = Uri.EscapeDataString(conta);
            // Formato otpauth padrão: rótulo "Emissor:conta" + issuer repetido no
            // query, digits/period explícitos para os apps mais antigos.
            return $"otpauth://totp/{e}:{c}?secret={secret}&issuer={e}&algorithm=SHA1&digits=6&period=30";
        }

        public bool Validar(string secret, string codigo)
        {
            if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(codigo))
                return false;

            byte[] bytes;
            try
            {
                bytes = Base32Encoding.ToBytes(secret);
            }
            catch
            {
                return false;
            }

            var totp = new Totp(bytes);
            // Tolera 1 passo (±30s) para o desvio de relógio do celular.
            return totp.VerifyTotp(
                codigo.Trim(),
                out _,
                new VerificationWindow(previous: 1, future: 1));
        }
    }
}
