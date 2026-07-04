using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : Base
    {
        private readonly TriboDeDaviContext _context;

        public BaseRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public virtual async Task<T> CreateAsync(T obj)
        {
            _context.Add(obj);
            await _context.SaveChangesAsync();

            return obj;
        }

        public virtual async Task DeleteAsync(long id)
        {
            var obj = await GetByIdAsync(id);

            if (obj != null)
            {
                _context.Remove(obj);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>()
                .AsNoTracking()
                .ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(long id)
        {
            var obj = await _context.Set<T>()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .ToListAsync();

            return obj.FirstOrDefault();
        }

        public virtual async Task<T> UpdateAsync(T obj)
        {
            // Verifica se já existe uma instância rastreada com o mesmo ID
            var local = _context.Set<T>().Local.FirstOrDefault(x => x.Id == obj.Id);
            if (local != null)
                _context.Entry(local).State = EntityState.Detached;

            _context.Entry(obj).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return obj;
        }
    }
}
