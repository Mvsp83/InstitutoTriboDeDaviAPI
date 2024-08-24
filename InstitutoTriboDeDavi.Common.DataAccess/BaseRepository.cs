using InstitutoTriboDeDavi.Common.BaseEntity.BaseEntity;
using InstitutoTriboDeDavi.Common.DataAccess.Interfaces;

namespace InstitutoTriboDeDavi.Common.DataAccess
{
    public class BaseRepository<T> : IBaseRepository<T> where T : Base
    {
        public Task<T> CreateAsync(T obj)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<T> UpdateAsync(T obj)
        {
            throw new NotImplementedException();
        }
    }
}
