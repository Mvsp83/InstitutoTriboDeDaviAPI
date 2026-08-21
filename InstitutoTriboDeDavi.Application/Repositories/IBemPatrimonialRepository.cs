using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IBemPatrimonialRepository : IBaseRepository<BemPatrimonial>
    {
        Task<List<BemPatrimonial>> ObterTodosAsync();
    }
}
