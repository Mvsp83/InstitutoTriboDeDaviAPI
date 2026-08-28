using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class FotoArquivoRepository : IFotoArquivoRepository
    {
        private readonly TriboDeDaviContext _context;

        public FotoArquivoRepository(TriboDeDaviContext context)
        {
            _context = context;
        }

        public async Task<FotoArquivo> AdicionarAsync(FotoArquivo arquivo)
        {
            _context.FotosArquivo.Add(arquivo);
            await _context.SaveChangesAsync();
            return arquivo;
        }

        public async Task<FotoArquivo> ObterAsync(long id)
        {
            return await _context.FotosArquivo
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task ExcluirAsync(long id)
        {
            var arquivo = await _context.FotosArquivo.FirstOrDefaultAsync(a => a.Id == id);
            if (arquivo != null)
            {
                _context.FotosArquivo.Remove(arquivo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
