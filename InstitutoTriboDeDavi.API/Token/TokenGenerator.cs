using InstitutoTriboDeDavi.API.Token.Interfaces;
using InstitutoTriboDeDavi.Application.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InstitutoTriboDeDavi.API.Token
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _configuration;

        public TokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(UsuarioDTO usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Login),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Role.ToString()),
                new Claim("PoloId", usuario.PoloId?.ToString() ?? string.Empty),
                new Claim("PoloNome", usuario.PoloNome ?? string.Empty),
                new Claim("PermiteGraduacao", usuario.PermiteGraduacao ? "true" : "false"),
                // Módulos comerciais contratados (lista separada por vírgula).
                new Claim("Modulos", ModulosResolver.Resolver(usuario, _configuration))
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:HoursToExpire"])),
                // Emitidos quando configurados (produção); validados no Startup
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateResponsavelToken(long alunoId, string nomeAluno)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, $"responsavel:{alunoId}"),
                // Papel próprio: não casa com Administrador/Supervisor/Professor,
                // então este token é inerte contra os endpoints internos.
                new Claim(ClaimTypes.Role, "Responsavel"),
                new Claim("AlunoId", alunoId.ToString()),
                new Claim("NomeAluno", nomeAluno ?? string.Empty),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                // Sessão curta: o portal é de consulta pontual.
                Expires = DateTime.UtcNow.AddHours(4),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
