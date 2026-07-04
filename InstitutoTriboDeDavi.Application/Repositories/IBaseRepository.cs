using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IBaseRepository<T> where T : Base
    {
        Task<T> CreateAsync(T obj);
        Task<T> UpdateAsync(T obj);
        Task DeleteAsync(long id);
        Task<T> GetByIdAsync(long id);
        Task<List<T>> GetAllAsync();
    }
}
