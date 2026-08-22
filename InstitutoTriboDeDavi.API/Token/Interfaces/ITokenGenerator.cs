using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.API.Token.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(UsuarioDTO usuarioDTO);

        // Token só-leitura do portal do responsável: escopado a um aluno (claim
        // AlunoId) e com papel "Responsavel", que nenhum endpoint interno aceita.
        string GenerateResponsavelToken(long alunoId, string nomeAluno);
    }
}
