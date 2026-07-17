using InstitutoTriboDeDavi.Application.DTO.Business;
using InstitutoTriboDeDavi.Application.DTO.Queries;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IAtividadeService
    {
        Task<AtividadeDTO> Create(AtividadeDTO atividadeDTO);
        Task<AtividadeDTO> Update(AtividadeDTO atividadeDTO);
        Task<AtividadeDTO> Get(long id);
        Task<List<AtividadeDTO>> GetAll();
        Task Delete(long id);
        Task<List<HistoricoAtividadeDTO>> ObterHistoricoTurmaAsync(long poloId, int turma);
    }
}
