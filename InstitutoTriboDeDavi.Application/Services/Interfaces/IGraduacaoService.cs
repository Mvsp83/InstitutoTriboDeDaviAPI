using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IGraduacaoService
    {
        Task<List<GraduacaoDTO>> Listar(int? ano, long? poloId);
        Task<List<GraduacaoDTO>> ListarPorAluno(long alunoId);
        Task<GraduacaoDTO> Obter(long id);
        Task<ResultadoGraduacaoDTO> Registrar(GraduacaoLoteDTO dto, string registradoPor);
        Task Excluir(long id);
        Task<List<AptidaoGraduacaoDTO>> ListarAptidao(long? poloId);
    }
}
