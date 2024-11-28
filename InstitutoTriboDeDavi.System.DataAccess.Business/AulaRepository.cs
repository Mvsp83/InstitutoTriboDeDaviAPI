using InstitutoTriboDeDavi.Common.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Business.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities.Business;
using InstitutoTriboDeDavi.System.Infra.Context;

namespace InstitutoTriboDeDavi.System.DataAccess.Business
{
    public class AulaRepository : BaseRepository<Aula>, IAulaRepository
    {
        private readonly TriboDeDaviContext _context;

        public AulaRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }    
    }
}
