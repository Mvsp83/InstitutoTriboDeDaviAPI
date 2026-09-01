using InstitutoTriboDeDavi.Infrastructure.Repositories;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        private readonly TriboDeDaviContext _context;
        public UsuarioRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Usuario> GetByEmail(string email)
        {
            var user = await _context.Usuarios
                .Where(c => c.Email.ToLower() == email.ToLower())
                .AsNoTracking()
                .ToListAsync();

            return user.FirstOrDefault();
        }

        public async Task<List<Usuario>> SearchByEmail(string email)
        {
            var allUsers = await _context.Usuarios
                .Where(c => c.Email.ToLower().Contains(email.ToLower()))
                .AsNoTracking()
                .ToListAsync();

            return allUsers;
        }

        public async Task<Usuario> GetByNome(string nome)
        {
            var user = await _context.Usuarios
                .Where(c => c.Login.ToLower() == nome.ToLower())
                .AsNoTracking()
                .ToListAsync();

            return user.FirstOrDefault();
        }

        public async Task<List<Usuario>> SearchByNome(string nome)
        {
            var allUsers = await _context.Usuarios
                .Where(c => c.Login.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToListAsync();

            return allUsers;
        }

        public async Task<Usuario> ObterUsuarioPorLoginAsync(string login)
        {
            // Case-insensitive: o SQL Server usava collation CI; o PostgreSQL é
            // case-sensitive, então normalizamos para não quebrar o login.
            var alvo = (login ?? string.Empty).ToLower();
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Login.ToLower() == alvo);
        }

        public async Task<List<Usuario>> ObterTodosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<List<Usuario>> ObterPorPoloTurmaAsync(long poloId, List<int> turmas)
        {
            return await _context.Usuarios
                                 .Where(a => a.PoloId == poloId)
                                 .ToListAsync();
        }

        public async Task<bool> ExisteQualquerUsuario()
        {
            return await _context.Usuarios.AnyAsync();
        }
    }
}
