using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Preferência de layout do Dashboard por usuário: quais widgets aparecem e
    // em que ordem. O layout é guardado como JSON (o portal é dono do formato),
    // então acrescentar widgets novos não exige alterar o banco.
    public class ConfiguracaoDashboard : Base
    {
        public string UsuarioLogin { get; set; }
        public string Layout { get; set; } = string.Empty;

        public override bool Validate()
        {
            _errors.Clear();

            if (string.IsNullOrWhiteSpace(UsuarioLogin))
                _errors.Add("O usuário da configuração não pode ser vazio.");
            else if (UsuarioLogin.Length > 120)
                _errors.Add("O usuário da configuração deve ter no máximo 120 caracteres.");

            if (Layout?.Length > 4000)
                _errors.Add("O layout do dashboard é grande demais.");

            if (_errors.Count > 0)
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
