using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class FotoTreinoRepository : IFotoTreinoRepository
    {
        private readonly TriboDeDaviContext _context;

        public FotoTreinoRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task<FotoTreino> ObterAsync(long id)
        {
            return await _context.FotosTreino
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<FotoTreino> ObterPorAulaAsync(long poloId, int turma, DateTime dataAula)
        {
            var dia = dataAula.Date;
            return await _context.FotosTreino
                .FirstOrDefaultAsync(f =>
                    f.PoloId == poloId && f.Turma == turma && f.DataAula == dia);
        }

        public async Task<FotoTreino> AdicionarAsync(FotoTreino foto)
        {
            _context.FotosTreino.Add(foto);
            await _context.SaveChangesAsync();
            return foto;
        }

        public async Task<FotoTreino> AtualizarAsync(FotoTreino foto)
        {
            _context.FotosTreino.Update(foto);
            await _context.SaveChangesAsync();
            return foto;
        }

        public async Task ExcluirAsync(long id)
        {
            var foto = await _context.FotosTreino.FirstOrDefaultAsync(f => f.Id == id);
            if (foto != null)
            {
                _context.FotosTreino.Remove(foto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DefinirPublicacaoAsync(long id, bool publicada)
        {
            var foto = await _context.FotosTreino.FirstOrDefaultAsync(f => f.Id == id);
            if (foto == null) return;
            foto.Publicada = publicada;
            await _context.SaveChangesAsync();
        }

        public async Task<List<(FotoTreino foto, string poloNome)>> ListarTodasAsync()
        {
            return await MontarListagem(_context.FotosTreino.AsNoTracking());
        }

        public async Task<List<(FotoTreino foto, string poloNome)>> ListarPublicasAsync()
        {
            return await MontarListagem(
                _context.FotosTreino.AsNoTracking().Where(f => f.Publicada));
        }

        // Junta a foto ao nome do polo, ordenando da aula mais recente para a mais antiga.
        private async Task<List<(FotoTreino, string)>> MontarListagem(IQueryable<FotoTreino> fonte)
        {
            var query =
                from f in fonte
                join p in _context.Polos.AsNoTracking() on f.PoloId equals p.Id into gp
                from p in gp.DefaultIfEmpty()
                orderby f.DataAula descending
                select new { Foto = f, Nome = p != null ? p.Nome : null };

            var lista = await query.ToListAsync();
            return lista.Select(x => (x.Foto, x.Nome)).ToList();
        }

        // ── Config por polo ───────────────────────────────────────────────
        public async Task<PoloFotoConfig> ObterConfigAsync(long poloId)
        {
            return await _context.PoloFotoConfigs
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.PoloId == poloId);
        }

        public async Task DefinirConfigAsync(long poloId, bool requerAutorizacao)
        {
            var config = await _context.PoloFotoConfigs.FirstOrDefaultAsync(c => c.PoloId == poloId);
            if (config == null)
            {
                _context.PoloFotoConfigs.Add(new PoloFotoConfig
                {
                    PoloId = poloId,
                    RequerAutorizacao = requerAutorizacao,
                });
            }
            else
            {
                config.RequerAutorizacao = requerAutorizacao;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<(long poloId, string poloNome, bool requerAutorizacao)>> ListarConfigPolosAsync()
        {
            var query =
                from p in _context.Polos.AsNoTracking()
                join c in _context.PoloFotoConfigs.AsNoTracking() on p.Id equals c.PoloId into gc
                from c in gc.DefaultIfEmpty()
                orderby p.Nome
                // Ausente = requer autorização (padrão seguro).
                select new { p.Id, p.Nome, Requer = c == null || c.RequerAutorizacao };

            var lista = await query.ToListAsync();
            return lista.Select(x => (x.Id, x.Nome, x.Requer)).ToList();
        }
    }
}
