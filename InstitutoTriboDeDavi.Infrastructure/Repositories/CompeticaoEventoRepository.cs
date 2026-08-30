using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class CompeticaoEventoRepository : ICompeticaoEventoRepository
    {
        private readonly TriboDeDaviContext _context;

        public CompeticaoEventoRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task<List<CompeticaoEvento>> ListarAsync()
        {
            return await _context.CompeticoesEvento
                .Include(c => c.Participacoes)
                .AsNoTracking()
                .OrderByDescending(c => c.Data)
                .ToListAsync();
        }

        public async Task<CompeticaoEvento> ObterComParticipacoesAsync(long id)
        {
            return await _context.CompeticoesEvento
                .Include(c => c.Participacoes)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CompeticaoEvento> CriarAsync(CompeticaoEvento evento)
        {
            _context.CompeticoesEvento.Add(evento);
            await _context.SaveChangesAsync();
            return evento;
        }

        public async Task AtualizarAsync(CompeticaoEvento evento)
        {
            var e = await _context.CompeticoesEvento.FirstOrDefaultAsync(x => x.Id == evento.Id);
            if (e == null) return;
            e.Nome = evento.Nome;
            e.Data = evento.Data;
            e.DataFim = evento.DataFim;
            e.Local = evento.Local;
            e.Organizador = evento.Organizador;
            e.PrazoInscricao = evento.PrazoInscricao;
            e.Link = evento.Link;
            e.Observacao = evento.Observacao;
            e.Status = evento.Status;
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var e = await _context.CompeticoesEvento.FirstOrDefaultAsync(x => x.Id == id);
            if (e != null) { _context.CompeticoesEvento.Remove(e); await _context.SaveChangesAsync(); }
        }

        public async Task<ParticipacaoAtleta> AdicionarParticipacaoAsync(ParticipacaoAtleta p)
        {
            _context.ParticipacoesAtleta.Add(p);
            await _context.SaveChangesAsync();
            return p;
        }

        public async Task<ParticipacaoAtleta> AtualizarParticipacaoAsync(ParticipacaoAtleta p)
        {
            var atual = await _context.ParticipacoesAtleta.FirstOrDefaultAsync(x => x.Id == p.Id);
            if (atual == null) return null;
            atual.CategoriaPeso = p.CategoriaPeso;
            atual.Colocacao = p.Colocacao;
            atual.Lutas = p.Lutas;
            atual.Vitorias = p.Vitorias;
            atual.Finalizacoes = p.Finalizacoes;
            atual.Observacao = p.Observacao;
            await _context.SaveChangesAsync();
            return atual;
        }

        public async Task<ParticipacaoAtleta> ObterParticipacaoAsync(long id)
        {
            return await _context.ParticipacoesAtleta.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task RemoverParticipacaoAsync(long id)
        {
            var p = await _context.ParticipacoesAtleta.FirstOrDefaultAsync(x => x.Id == id);
            if (p != null) { _context.ParticipacoesAtleta.Remove(p); await _context.SaveChangesAsync(); }
        }
    }
}
