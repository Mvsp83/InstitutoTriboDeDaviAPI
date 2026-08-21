using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class BemPatrimonialRepository
        : BaseRepository<BemPatrimonial>, IBemPatrimonialRepository
    {
        private readonly TriboDeDaviContext _context;

        public BemPatrimonialRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<BemPatrimonial>> ObterTodosAsync()
        {
            return await _context.BensPatrimoniais
                .AsNoTracking()
                .OrderBy(b => b.Categoria)
                .ThenBy(b => b.Descricao)
                .ToListAsync();
        }
    }
}
