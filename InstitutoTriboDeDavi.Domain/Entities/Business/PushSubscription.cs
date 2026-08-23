using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities.Business
{
    // Inscrição de Web Push de um dispositivo. Cada navegador/dispositivo que o
    // usuário autoriza gera um endpoint único (fornecido pelo push service do
    // navegador) com as chaves p256dh/auth usadas para criptografar a mensagem.
    public class PushSubscription : Base
    {
        // Login do usuário dono da inscrição (a quem as notificações se destinam).
        public string UsuarioLogin { get; set; }
        // Endpoint do push service (URL longa e única por dispositivo).
        public string Endpoint { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }
        public DateTime DataCriacao { get; set; }

        // Sem regras de negócio próprias — os dados vêm do navegador e são
        // validados no controller (campos obrigatórios).
        public override bool Validate() => true;
    }
}
