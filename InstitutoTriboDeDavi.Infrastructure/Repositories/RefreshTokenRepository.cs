using System;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly TriboDeDaviContext _context;

        public RefreshTokenRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task CriarAsync(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> ObterPorHashAsync(string tokenHash)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
        }

        public async Task RevogarAsync(RefreshToken token)
        {
            token.RevogadoEm = DateTime.UtcNow;
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }

        public async Task<int> RevogarTodosDoUsuarioAsync(long usuarioId)
        {
            var ativos = await _context.RefreshTokens
                .Where(t => t.UsuarioId == usuarioId && t.RevogadoEm == null)
                .ToListAsync();

            foreach (var t in ativos)
                t.RevogadoEm = DateTime.UtcNow;

            if (ativos.Count > 0)
                await _context.SaveChangesAsync();

            return ativos.Count;
        }
    }
}
