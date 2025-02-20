using InstitutoTriboDeDavi.API.Utilities.Interfaces;

namespace InstitutoTriboDeDavi.API.Utilities
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly Microsoft.AspNetCore.Identity.PasswordHasher<object> _hasher;

        public PasswordHasher()
        {
            _hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
        }

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var result = _hasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
            return result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success;
        }
    }

}
