using InstitutoTriboDeDavi.Infrastructure.Repositories;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoTriboDeDavi.Infrastructure.Repositories
{
    public class AlunoRepository : BaseRepository<Aluno>, IAlunoRepository
    {
        private readonly TriboDeDaviContext _context;
        public AlunoRepository(TriboDeDaviContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Aluno> GetByNome(string nome)
        {
            var aluno = await _context.Alunos
                .Where(c => c.Nome.ToLower() == nome.ToLower())
                .AsNoTracking()
                .ToListAsync();

            return aluno.FirstOrDefault();
        }

        // Busca por CPF (identidade forte — evita que homônimos se sobrescrevam
        // na sincronização). Compara pelo valor da planilha, que é a mesma
        // origem/formatação com que o CPF foi gravado. Se o formato do CPF no
        // formulário mudar, o import cai no fallback por nome (comportamento
        // anterior), nunca pior que hoje.
        public async Task<Aluno> GetByCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return null;

            var alvo = cpf.Trim();
            var alunos = await _context.Alunos
                .Where(a => a.CPF == alvo)
                .AsNoTracking()
                .ToListAsync();

            return alunos.FirstOrDefault();
        }

        public async Task<List<Aluno>> SearchByNome(string nome)
        {
            var allAlunos = await _context.Alunos
                .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
                .AsNoTracking()
                .ToListAsync();

            return allAlunos;
        }

        public async Task<int> GetTotalAlunosAsync()
        {
            return await _context.Set<Aluno>().CountAsync();
        }

        public async Task<List<Aluno>> ObterTodosAsync()
        {
            return await _context.Alunos.ToListAsync();
        }

        public async Task<List<Aluno>> ObterPorPoloTurmaAsync(long poloId, List<int> turmas)
        {
            return await _context.Alunos
                                 .Where(a => a.PoloId == poloId && turmas.Contains(a.Turma))
                                 .ToListAsync();
        }

        public async Task<List<Aluno>> ObterPendentesPorPoloAsync(long poloId)
        {
            return await _context.Alunos
                .Where(a => a.PoloId == poloId && a.Turma == 0)
                .OrderBy(a => a.Nome)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Aluno>> ObterTodosPendentesAsync()
        {
            return await _context.Alunos
                .Where(a => a.Turma == 0)
                .OrderBy(a => a.PoloId)
                .ThenBy(a => a.Nome)
                .AsNoTracking()
                .ToListAsync();
        }

        // ── LGPD ──────────────────────────────────────────────────────────

        public async Task<DadosPessoaisAluno> ColetarDadosPessoaisAsync(long alunoId)
        {
            var aluno = await _context.Alunos.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == alunoId);

            if (aluno == null)
                return null;

            var matriculas = await _context.Matriculas.AsNoTracking()
                .Where(m => m.AlunoId == alunoId)
                .OrderByDescending(m => m.Ano)
                .ToListAsync();

            var graduacoes = await _context.Graduacoes.AsNoTracking()
                .Where(g => g.AlunoId == alunoId)
                .OrderByDescending(g => g.Data)
                .ToListAsync();

            var presencas = await _context.Presencas.AsNoTracking()
                .Where(p => p.AlunoId == alunoId)
                .OrderByDescending(p => p.Data)
                .ToListAsync();

            var inscricoes = await _context.Inscricoes.AsNoTracking()
                .Where(i => i.AlunoId == alunoId)
                .OrderByDescending(i => i.DataEnvio)
                .ToListAsync();

            return new DadosPessoaisAluno(aluno, matriculas, graduacoes, presencas, inscricoes);
        }

        public async Task<Aluno> AnonimizarAsync(long alunoId)
        {
            var aluno = await _context.Alunos.FirstOrDefaultAsync(a => a.Id == alunoId);

            if (aluno == null)
                return null;

            // Idempotente: reexecutar não muda nada nem gera novo log.
            if (aluno.AnonimizadoEm != null)
                return aluno;

            var marcador = $"Aluno anonimizado #{aluno.Id}";

            // Aluno — apaga os identificadores diretos. Mantém polo, turma,
            // faixa e data de nascimento: sem nome/CPF eles deixam de
            // identificar a pessoa e ainda alimentam as estatísticas de editais.
            aluno.Nome = marcador;
            aluno.RG = null;
            aluno.CPF = null;
            aluno.Endereco = null;
            aluno.Numero = null;
            aluno.Complemento = null;
            aluno.Bairro = null;
            aluno.Cidade = null;
            aluno.Celular = null;
            aluno.Telefone2 = null;
            aluno.Responsavel = null;
            aluno.Parentesco = null;
            aluno.RGResponsavel = null;
            aluno.CPFResponsavel = null;
            aluno.Escola = null;
            aluno.Serie = null;
            aluno.Periodo = null;
            aluno.AnonimizadoEm = DateTime.Now;

            // Presenças copiam o nome do aluno — o nome precisa sumir aqui também.
            var presencas = await _context.Presencas
                .Where(p => p.AlunoId == alunoId)
                .ToListAsync();
            foreach (var p in presencas)
            {
                p.NomeAluno = marcador;
            }

            // Inscrições — apaga a PII, mas preserva os aceites, a versão dos
            // termos e a data como prova de que houve consentimento (registro
            // das operações de tratamento).
            var inscricoes = await _context.Inscricoes
                .Where(i => i.AlunoId == alunoId)
                .ToListAsync();
            foreach (var i in inscricoes)
            {
                i.Nome = marcador;
                i.Rg = string.Empty;
                i.Cpf = string.Empty;
                i.Escola = string.Empty;
                i.Serie = string.Empty;
                i.Periodo = string.Empty;
                i.NomeResponsavel = string.Empty;
                i.RgResponsavel = string.Empty;
                i.CpfResponsavel = string.Empty;
                i.ParentescoOutro = string.Empty;
                i.Rua = string.Empty;
                i.Numero = string.Empty;
                i.Complemento = string.Empty;
                i.Bairro = string.Empty;
                i.Cidade = string.Empty;
                i.WhatsApp = string.Empty;
                i.Telefone2 = string.Empty;
                i.Medicamentos = string.Empty;
                i.RespostasSaudeJson = string.Empty;
                i.RespostasFamiliarJson = string.Empty;
                i.NomeAssinatura = marcador;
            }

            // Uma só transação (SaveChanges) cobre aluno + presenças + inscrições.
            await _context.SaveChangesAsync();

            return aluno;
        }

        public async Task<List<CandidatoRetencao>> ObterCandidatosRetencaoAsync(int mesesInativo)
        {
            var limite = DateTime.Now.AddMonths(-mesesInativo);
            var candidatos = new List<CandidatoRetencao>();

            var alunos = await _context.Alunos.AsNoTracking()
                .Where(a => a.AnonimizadoEm == null)
                .ToListAsync();

            foreach (var a in alunos)
            {
                DateTime? ultimaPresenca = await _context.Presencas.AsNoTracking()
                    .Where(p => p.AlunoId == a.Id)
                    .OrderByDescending(p => p.Data)
                    .Select(p => (DateTime?)p.Data)
                    .FirstOrDefaultAsync();

                int? ultimoAno = await _context.Matriculas.AsNoTracking()
                    .Where(m => m.AlunoId == a.Id)
                    .OrderByDescending(m => m.Ano)
                    .Select(m => (int?)m.Ano)
                    .FirstOrDefaultAsync();

                // Ativo se teve presença após o limite, ou matrícula num ano cujo
                // encerramento (31/12) ainda é posterior ao limite.
                var ativoPorPresenca = ultimaPresenca.HasValue && ultimaPresenca.Value >= limite;
                var ativoPorMatricula = ultimoAno.HasValue && new DateTime(ultimoAno.Value, 12, 31) >= limite;
                if (ativoPorPresenca || ativoPorMatricula)
                    continue;

                var referencia = ultimaPresenca
                    ?? (ultimoAno.HasValue ? new DateTime(ultimoAno.Value, 12, 31) : (DateTime?)null);
                var meses = referencia.HasValue
                    ? (int)((DateTime.Now - referencia.Value).TotalDays / 30)
                    : mesesInativo;

                candidatos.Add(new CandidatoRetencao(a, ultimaPresenca, ultimoAno, meses));
            }

            return candidatos.OrderByDescending(c => c.MesesInativo).ToList();
        }

        public async Task<Aluno> ObterPorCodigoResponsavelAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            var alvo = codigo.Trim();
            return await _context.Alunos
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.CodigoResponsavel == alvo && a.AnonimizadoEm == null);
        }

        public async Task<List<Aluno>> ObterPorPoloAsync(long poloId)
        {
            return await _context.Alunos
                .Where(a => a.PoloId == poloId && a.AnonimizadoEm == null)
                .OrderBy(a => a.Nome)
                .ToListAsync();
        }
    }
}
