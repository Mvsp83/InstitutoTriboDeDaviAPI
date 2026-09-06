using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Configuração única (singleton) da loja: número de WhatsApp usado nos
    // pedidos e se o botão "Comprar via WhatsApp" aparece no site público.
    public class ConfiguracaoLoja : Base
    {
        public bool CompraWhatsappHabilitada { get; set; }
        // Só dígitos, com DDD (ex.: "47999998888"). Vazio = sem número.
        public string WhatsappNumero { get; set; } = string.Empty;

        public override bool Validate()
        {
            _errors.Clear();

            if (WhatsappNumero?.Length > 20)
                _errors.Add("O número de WhatsApp é grande demais.");

            if (_errors.Count > 0)
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
