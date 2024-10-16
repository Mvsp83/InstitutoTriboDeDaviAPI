using InstitutoTriboDeDavi.Common.DataAccess;
using InstitutoTriboDeDavi.System.DataAccess.Interfaces;
using InstitutoTriboDeDavi.System.Domain.Entities;
using InstitutoTriboDeDavi.System.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.System.DataAccess
{
    public class EnderecoRepository : BaseRepository<Endereco>, IEnderecoRepository
    {
        private readonly TriboDeDaviContext _context;
        public EnderecoRepository(TriboDeDaviContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<Endereco> GetByNome(string nome)
        {
            var endereco = await _context.Enderecos
                .Where(c => c.Logradouro.ToLower() == nome.ToLower())
                .AsNoTracking()
                .ToListAsync();

            return endereco.FirstOrDefault();
        }

        public async Task<List<Endereco>> SearchByNome(string nome)
        {
            var allEnderecos = await _context.Enderecos
                .Where(c => c.Logradouro.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToListAsync();

            return allEnderecos;
        }
    }
}
