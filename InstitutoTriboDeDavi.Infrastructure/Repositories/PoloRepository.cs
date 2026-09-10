using InstitutoTriboDeDavi.Infrastructure.Repositories;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class PoloRepository : BaseRepository<Polo>, IPoloRepository
    {
        private readonly TriboDeDaviContext _context;
        public PoloRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Polo> GetByNome(string nome)
        {
            var polo = await _context.Polos
                .Where(c => c.Nome.ToLower() == nome.ToLower())
                .AsNoTracking()
                .ToListAsync();

            return polo.FirstOrDefault();
        }

        public async Task<List<Polo>> SearchByNome(string nome)
        {
            var allPolos = await _context.Polos
                .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToListAsync();

            return allPolos;
        }

        public async Task<List<Polo>> ObterTodosAsync()
        {
            // Inclui os horários por turma — é a listagem que a tela de polos usa.
            return await _context.Polos
                .Include(p => p.Horarios)
                .AsNoTracking()
                .ToListAsync();
        }

        // Sobrescreve os GETs base para trazer os horários por turma junto.
        public override async Task<List<Polo>> GetAllAsync()
        {
            return await _context.Polos
                .Include(p => p.Horarios)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Polo> GetByIdAsync(long id)
        {
            return await _context.Polos
                .Include(p => p.Horarios)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Atualiza os campos do polo e troca os horários (estratégia replace):
        // remove os antigos e insere os novos vindos do formulário.
        public async Task<Polo> AtualizarComHorariosAsync(Polo polo)
        {
            var existente = await _context.Polos
                .Include(p => p.Horarios)
                .FirstOrDefaultAsync(p => p.Id == polo.Id);

            if (existente == null)
                return null;

            existente.Nome = polo.Nome;
            existente.Informacoes = polo.Informacoes;
            existente.Endereco = polo.Endereco;
            existente.Bairro = polo.Bairro;
            existente.Cidade = polo.Cidade;
            existente.LimiteAlunos = polo.LimiteAlunos;
            existente.AceitaAdultos = polo.AceitaAdultos;

            _context.HorariosTurma.RemoveRange(existente.Horarios);
            foreach (var h in polo.Horarios ?? new List<HorarioTurma>())
            {
                _context.HorariosTurma.Add(new HorarioTurma
                {
                    PoloId = existente.Id,
                    Turma = h.Turma,
                    DiaSemana = h.DiaSemana,
                    HoraInicio = h.HoraInicio,
                    HoraFim = h.HoraFim,
                });
            }

            await _context.SaveChangesAsync();

            // Recarrega com os horários recém-gravados para a resposta refletir
            // o estado real (o SaveChanges acima não repovoa a navegação).
            return await _context.Polos
                .Include(p => p.Horarios)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == existente.Id);
        }
    }
}
