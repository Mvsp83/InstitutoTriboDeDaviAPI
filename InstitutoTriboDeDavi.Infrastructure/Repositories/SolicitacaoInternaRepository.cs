using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class SolicitacaoInternaRepository
        : BaseRepository<SolicitacaoInterna>, ISolicitacaoInternaRepository
    {
        private readonly TriboDeDaviContext _context;

        public SolicitacaoInternaRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<SolicitacaoInterna>> ListarTodasAsync()
        {
            return await _context.SolicitacoesInternas
                .AsNoTracking()
                .OrderByDescending(s => s.DataAtualizacao)
                .ToListAsync();
        }

        public async Task<List<SolicitacaoInterna>> ListarDoUsuarioAsync(string login)
        {
            return await _context.SolicitacoesInternas
                .Where(s => s.CriadoPorLogin == login || s.DestinatarioLogin == login)
                .AsNoTracking()
                .OrderByDescending(s => s.DataAtualizacao)
                .ToListAsync();
        }

        public async Task<SolicitacaoInterna> ObterComMensagensAsync(long id)
        {
            return await _context.SolicitacoesInternas
                .Include(s => s.Mensagens.OrderBy(m => m.DataEnvio))
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AdicionarMensagemAsync(MensagemSolicitacao mensagem)
        {
            _context.MensagensSolicitacao.Add(mensagem);
            await _context.SaveChangesAsync();
        }

        public async Task<int> ContarNaoResolvidasTodasAsync()
        {
            return await _context.SolicitacoesInternas
                .CountAsync(s => s.Ativo && s.Status != (int)StatusSolicitacao.Resolvida);
        }

        public async Task<int> ContarNaoResolvidasDoUsuarioAsync(string login)
        {
            return await _context.SolicitacoesInternas
                .CountAsync(s => s.Ativo
                    && s.Status != (int)StatusSolicitacao.Resolvida
                    && (s.CriadoPorLogin == login || s.DestinatarioLogin == login));
        }
    }
}
