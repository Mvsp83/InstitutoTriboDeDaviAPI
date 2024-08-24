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
            var user = await _context.Users
                .Where(c => c.Email.ToLower() == email.ToLower())
                .AsNoTracking()
                .ToListAsync();

            return user.FirstOrDefault();
        }

        public async Task<List<Usuario>> SearchEmail(string email)
        {
            var allUsers = await _context.Users
                .Where(c => c.Email.ToLower().Contains(email.ToLower()))
                .AsNoTracking()
                .ToListAsync();

            return allUsers;
        }
    }
}
