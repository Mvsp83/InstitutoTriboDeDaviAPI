using System;

namespace InstitutoTriboDeDavi.API.ViewModels.Create
{
    // Credenciais do portal do responsável: o código de acesso do aluno e a
    // data de nascimento dele (segundo fator leve).
    public class AcessoResponsavelViewModel
    {
        public string Codigo { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
    }
}
