using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IBemPatrimonialService
    {
        Task<List<BemPatrimonialDTO>> GetAll();
        Task<BemPatrimonialDTO> Get(long id);
        Task<BemPatrimonialDTO> Create(BemPatrimonialDTO dto);
        Task<BemPatrimonialDTO> Update(BemPatrimonialDTO dto);
        Task Delete(long id);
    }
}
