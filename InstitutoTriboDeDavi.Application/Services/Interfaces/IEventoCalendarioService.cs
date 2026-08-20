using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IEventoCalendarioService
    {
        Task<List<EventoCalendarioDTO>> ObterPorAno(int ano);
        Task<List<int>> ObterAnos();
        Task<EventoCalendarioDTO> Create(EventoCalendarioDTO dto);
        Task<EventoCalendarioDTO> Update(EventoCalendarioDTO dto);
        Task Delete(long id);
        // Clona todos os eventos de um ano para outro (mesmo mês/dia), retornando
        // quantos foram criados.
        Task<int> CopiarAno(int anoOrigem, int anoDestino);
    }
}
