using System;
using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Token de renovação da sessão. Fica no servidor para poder ser revogado
    // (desativar usuário, "sair de todos os aparelhos") — o access token JWT é
    // stateless e não dá para invalidar antes de expirar; o refresh é o ponto
    // de controle. Guardamos só o HASH do token, nunca o valor em claro.
    public class RefreshToken : Base
    {
        public long UsuarioId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; }
        public DateTime ExpiraEm { get; set; }
        // Preenchido quando o token é rotacionado (no refresh) ou revogado.
        public DateTime? RevogadoEm { get; set; }

        public bool Ativo => RevogadoEm == null && ExpiraEm > DateTime.UtcNow;

        // Entidade interna de infraestrutura: sem regras de validação de domínio.
        public override bool Validate() => true;
    }
}
