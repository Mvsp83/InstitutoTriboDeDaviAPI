using System;
using System.Threading.Tasks;
using InstitutoTriboDeDavi.Application.DTO;

namespace InstitutoTriboDeDavi.Application.Services.Interfaces
{
    public interface IResponsavelService
    {
        // Valida código + data de nascimento. Retorna os dados do aluno para o
        // controller emitir o token, ou null se as credenciais não conferem.
        Task<AcessoResponsavelDTO> AutenticarAsync(string codigo, DateTime dataNascimento);

        // Monta o painel só-leitura do aluno. Null se o aluno não existe.
        Task<PainelResponsavelDTO> ObterPainelAsync(long alunoId);

        // O responsável autoriza (ou revoga) o uso de imagem/voz do aluno,
        // registrando a data do consentimento. Retorna o novo estado.
        Task<bool> AtualizarAutorizacaoImagemAsync(long alunoId, bool autoriza);
    }
}
