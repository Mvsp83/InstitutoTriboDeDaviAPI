using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Validators;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    public class Usuario : Base
    {
        public string Email { get; set; }
        public string Login { get; set; }
        public string SenhaHash { get; set; }
        public UserRole Role { get; set; }
        public long? PoloId { get; set; }
        public string PoloNome { get; set; }

        // Permissão extra: libera o acesso do professor ao módulo Programa de
        // Graduação (posições, programas e golpes restritos). Admin já tem acesso.
        public bool PermiteGraduacao { get; set; }

        // Avatar do usuário: chave curta de preset ("preset:7") ou miniatura em
        // data URI. Anulável = sem avatar (a UI cai nas iniciais).
        public string? Avatar { get; set; }

        // Perfil público do professor (seção "Polos e Endereços" do site).
        // Nome de exibição, faixa (base 0..40, mesmo sistema dos alunos), foto
        // de rosto profissional (data URI) e o opt-in de aparecer no site.
        public string? Nome { get; set; }
        public int? Faixa { get; set; }
        public string? FotoSite { get; set; }
        public bool MostrarNoSite { get; set; }

        // Autenticação em dois fatores (TOTP). Secret em base32; nulo = sem 2FA.
        // Confirmado só depois que o usuário valida o primeiro código — antes
        // disso o secret existe mas o login ainda não exige o segundo fator.
        public string? TotpSecret { get; set; }
        public bool TotpConfirmado { get; set; }

        public override bool Validate()
        {
            var validator = new UsuarioValidator();
            var validation = validator.Validate(this);

            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                {
                    _errors.Add(error.ErrorMessage);
                }

                throw new DomainException("Alguns campos estão inválidos!", _errors);
            }

            return true;
        }
    }
}
