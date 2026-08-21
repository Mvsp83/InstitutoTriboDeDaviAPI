using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class AvisoRepository : BaseRepository<Aviso>, IAvisoRepository
    {
        private readonly TriboDeDaviContext _context;

        public AvisoRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Aviso>> ObterTodosAsync()
        {
            return await _context.Avisos
                .AsNoTracking()
                .OrderByDescending(a => a.DataCriacao)
                .ToListAsync();
        }

        public async Task<List<Aviso>> ObterAtivosAsync()
        {
            return await _context.Avisos
                .Where(a => a.Ativo)
                .AsNoTracking()
                .OrderByDescending(a => a.DataCriacao)
                .ToListAsync();
        }

        public async Task<List<long>> ObterCientesDoUsuarioAsync(string login)
        {
            return await _context.AvisosCientes
                .Where(c => c.UsuarioLogin == login)
                .Select(c => c.AvisoId)
                .ToListAsync();
        }

        public async Task RegistrarCienteAsync(long avisoId, string login)
        {
            var jaExiste = await _context.AvisosCientes
                .AnyAsync(c => c.AvisoId == avisoId && c.UsuarioLogin == login);
            if (jaExiste) return;

            _context.AvisosCientes.Add(new AvisoCiente
            {
                AvisoId = avisoId,
                UsuarioLogin = login,
                DataCiente = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }
    }
}
