using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    // Resultado do envio público: o id e o código de acesso que a família
    // recebe no fim da ficha para acompanhar o aluno no portal.
    public record EnvioInscricaoResultado(long Id, string CodigoResponsavel);

    // Resultado da matrícula em lote (virada de ano): quantas foram criadas,
    // quantas já existiam e o total de alunos considerados.
    public record MatriculaLoteResultado(int Criadas, int JaMatriculados, int TotalAlunos);

    public interface IInscricaoService
    {
        // Envio público (sem autenticação).
        Task<EnvioInscricaoResultado> Enviar(InscricaoDTO dto);

        // Rematrícula pública: busca um aluno já cadastrado por CPF do responsável
        // + data de nascimento do aluno (2 fatores, como o portal) para pré-
        // preencher o formulário. Null se não encontrar (mesma resposta que erro,
        // para não permitir enumeração de CPFs).
        Task<DadosPreMatriculaDTO> BuscarParaRematricula(string cpfResponsavel, DateTime dataNascimento);

        // Fila de revisão. poloId nulo = todos (Administrador).
        Task<List<InscricaoDTO>> Listar(int? status, int? ano, long? poloId);
        Task<InscricaoDTO> Obter(long id);
        Task<int> ContarPendentes(long? poloId);

        // Aprovar cria/atualiza o aluno e a matrícula do ano; o revisor pode
        // corrigir polo e turma nesse momento.
        Task<MatriculaDTO> Aprovar(long id, RevisaoInscricaoDTO revisao, string revisor);
        Task Recusar(long id, string motivo, string revisor);

        Task<List<MatriculaDTO>> ListarMatriculas(int ano, long? poloId);

        // Liga/desliga a matrícula do aluno no ano (inativar libera vaga;
        // reativar reocupa, respeitando o limite do polo).
        Task<MatriculaDTO> AlterarAtivaMatricula(long matriculaId, bool ativa);

        // Virada de ano: matricula em lote os alunos ativos ainda sem matrícula
        // no ano (polo/turma do cadastro atual). Idempotente. poloId nulo = todos.
        Task<MatriculaLoteResultado> MatricularAno(int ano, long? poloId);
    }
}
