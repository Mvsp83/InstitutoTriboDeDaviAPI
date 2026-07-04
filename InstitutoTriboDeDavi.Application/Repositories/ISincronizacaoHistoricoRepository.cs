using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface ISincronizacaoHistoricoRepository : IBaseRepository<SincronizacaoHistorico>
    {
        Task<List<SincronizacaoHistorico>> ObterUltimasAsync(int quantidade = 50);
        Task<List<SincronizacaoHistorico>> ObterPorPoloAsync(long poloId, int quantidade = 20);
        Task<SincronizacaoHistorico> ObterUltimaExecucaoAsync();
    }
}
