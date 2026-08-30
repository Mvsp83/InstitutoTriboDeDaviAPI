using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class AtletaRepository : IAtletaRepository
    {
        private readonly TriboDeDaviContext _context;

        public AtletaRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task<List<Atleta>> ListarAsync()
        {
            return await _context.Atletas
                .AsNoTracking()
                .OrderByDescending(a => a.DataInclusao)
                .ToListAsync();
        }

        public async Task<Atleta> ObterComTudoAsync(long id)
        {
            return await _context.Atletas
                .Include(a => a.Avaliacoes.OrderBy(av => av.Data))
                    .ThenInclude(av => av.Indicadores)
                .Include(a => a.Competicoes.OrderByDescending(c => c.Data))
                .Include(a => a.Anotacoes.OrderByDescending(an => an.Data))
                .Include(a => a.Metas)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Atleta> ObterPorAlunoAsync(long alunoId)
        {
            return await _context.Atletas
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AlunoId == alunoId);
        }

        public async Task<Atleta> CriarAsync(Atleta atleta)
        {
            _context.Atletas.Add(atleta);
            await _context.SaveChangesAsync();
            return atleta;
        }

        public async Task AtualizarPerfilAsync(Atleta atleta)
        {
            var existente = await _context.Atletas.FirstOrDefaultAsync(a => a.Id == atleta.Id);
            if (existente == null) return;
            existente.CategoriaPeso = atleta.CategoriaPeso;
            existente.Objetivo = atleta.Objetivo;
            existente.Status = atleta.Status;
            existente.Ativo = atleta.Ativo;
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var atleta = await _context.Atletas.FirstOrDefaultAsync(a => a.Id == id);
            if (atleta != null)
            {
                _context.Atletas.Remove(atleta);
                await _context.SaveChangesAsync();
            }
        }

        // ── Filhos ───────────────────────────────────────────────────────────

        public async Task<AvaliacaoFisica> AdicionarAvaliacaoAsync(AvaliacaoFisica avaliacao)
        {
            _context.AvaliacoesFisicas.Add(avaliacao);
            await _context.SaveChangesAsync();
            return avaliacao;
        }

        public async Task RemoverAvaliacaoAsync(long id)
        {
            var a = await _context.AvaliacoesFisicas.FirstOrDefaultAsync(x => x.Id == id);
            if (a != null) { _context.AvaliacoesFisicas.Remove(a); await _context.SaveChangesAsync(); }
        }

        public async Task<Competicao> AdicionarCompeticaoAsync(Competicao competicao)
        {
            _context.Competicoes.Add(competicao);
            await _context.SaveChangesAsync();
            return competicao;
        }

        public async Task RemoverCompeticaoAsync(long id)
        {
            var c = await _context.Competicoes.FirstOrDefaultAsync(x => x.Id == id);
            if (c != null) { _context.Competicoes.Remove(c); await _context.SaveChangesAsync(); }
        }

        public async Task<AnotacaoAtleta> AdicionarAnotacaoAsync(AnotacaoAtleta anotacao)
        {
            _context.AnotacoesAtleta.Add(anotacao);
            await _context.SaveChangesAsync();
            return anotacao;
        }

        public async Task RemoverAnotacaoAsync(long id)
        {
            var a = await _context.AnotacoesAtleta.FirstOrDefaultAsync(x => x.Id == id);
            if (a != null) { _context.AnotacoesAtleta.Remove(a); await _context.SaveChangesAsync(); }
        }

        public async Task<MetaAtleta> AdicionarMetaAsync(MetaAtleta meta)
        {
            _context.MetasAtleta.Add(meta);
            await _context.SaveChangesAsync();
            return meta;
        }

        public async Task<MetaAtleta> AtualizarMetaAsync(long id, int status)
        {
            var meta = await _context.MetasAtleta.FirstOrDefaultAsync(x => x.Id == id);
            if (meta == null) return null;
            meta.Status = status;
            meta.DataConclusao = status == (int)StatusMeta.Concluida ? DateTime.Now : null;
            await _context.SaveChangesAsync();
            return meta;
        }

        public async Task RemoverMetaAsync(long id)
        {
            var m = await _context.MetasAtleta.FirstOrDefaultAsync(x => x.Id == id);
            if (m != null) { _context.MetasAtleta.Remove(m); await _context.SaveChangesAsync(); }
        }
    }
}
