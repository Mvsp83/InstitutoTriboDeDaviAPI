using System;
using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    // Quem doa para o instituto. Manter o cadastro separado das doações permite
    // ver o histórico de cada pessoa e reconhecer quem sustenta o projeto ao
    // longo do tempo — que é o objetivo de um cadastro de doadores.
    public class Doador : Base
    {
        // 0 = pessoa física, 1 = pessoa jurídica.
        public int TipoPessoa { get; set; }
        public string Nome { get; set; } = string.Empty;
        // CPF ou CNPJ, conforme o tipo. Necessário no recibo de dedutibilidade.
        public string Documento { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;

        public override bool Validate()
        {
            _errors.Clear();

            if (string.IsNullOrWhiteSpace(Nome))
                _errors.Add("O nome do doador é obrigatório.");
            else if (Nome.Length > 160)
                _errors.Add("O nome deve ter no máximo 160 caracteres.");

            if (TipoPessoa != 0 && TipoPessoa != 1)
                _errors.Add("O tipo de pessoa é inválido.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
