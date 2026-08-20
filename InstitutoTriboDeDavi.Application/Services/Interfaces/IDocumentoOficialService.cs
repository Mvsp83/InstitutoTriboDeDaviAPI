using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IDocumentoOficialService
    {
        Task<List<DocumentoOficialDTO>> ObterPorAno(int ano);
        Task<List<int>> ObterAnos();
        Task<DocumentoOficialDTO> Get(long id);
        Task<DocumentoOficialDTO> Create(DocumentoOficialDTO dto);
        Task<DocumentoOficialDTO> Update(DocumentoOficialDTO dto);
        Task Delete(long id);
        Task<DocumentoOficialDTO> Aprovar(long id);
    }
}
