using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Domain.Entities;

namespace InstitutoTriboDeDavi.Application.Repositories
{
    public interface IGraduacaoRepository
    {
        Task<List<Graduacao>> ListarAsync(int? ano, long? poloId);
        Task<List<Graduacao>> ListarPorAlunoAsync(long alunoId);
        Task<Graduacao> ObterAsync(long id);

        // Grava as graduações e atualiza a faixa de cada aluno numa transação:
        // registro e faixa atual não podem divergir.
        Task<int> RegistrarAsync(List<(Graduacao graduacao, Aluno aluno)> itens);

        // Excluir devolve o aluno à faixa anterior — é o caminho de correção de
        // um registro lançado errado.
        Task ExcluirAsync(Graduacao graduacao, Aluno aluno);
    }
}
