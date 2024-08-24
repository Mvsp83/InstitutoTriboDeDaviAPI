using InstitutoTriboDeDavi.Common.BaseEntity.BaseEntity;

namespace InstitutoTriboDeDavi.Common.DataAccess.Interfaces
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
