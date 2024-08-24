using InstitutoTriboDeDavi.Common.BaseEntity.BaseEntity;
using InstitutoTriboDeDavi.Common.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Common.DataAccess
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
            _context.Entry(obj).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return obj;
        }
    }
}
