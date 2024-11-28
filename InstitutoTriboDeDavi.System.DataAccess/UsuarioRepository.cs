using InstitutoTriboDeDavi.Common.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.System.DataAccess
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
    }
}
