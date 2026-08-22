using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Domain.Validators;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    public class Aluno : Base
    {
        public string Nome { get; set; }
        public string? RG { get; set; }
        public string? CPF { get; set; }
        public DateTime DataNascimento { get; set; }
        public double? Peso { get; set; }
        public double? Altura { get; set; }
        public Faixa Faixa { get; set; }
        // Endereco guarda a rua/logradouro. Numero e Complemento viraram campos
        // próprios junto com a ficha de inscrição online, que já os recebe
        // separados; cadastros antigos podem ter o endereço inteiro em Endereco.
        public string? Endereco { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Celular { get; set; }
        public string? Telefone2 { get; set; }
        public string? Responsavel { get; set; }
        public Parentesco? Parentesco { get; set; }
        public string? RGResponsavel { get; set; }
        public string? CPFResponsavel { get; set; }
        public string? Escola { get; set; }
        public string? Serie { get; set; }
        public string? Periodo { get; set; }
        public long PoloId { get; set; }
        public int Turma { get; set; }
        // Marca a eliminação dos dados pessoais (LGPD). Quando preenchida, os
        // identificadores diretos foram apagados e o registro só permanece,
        // anonimizado, para sustentar a prestação de contas (art. 16).
        public DateTime? AnonimizadoEm { get; set; }

        // Código de acesso do responsável ao portal de acompanhamento. Gerado
        // pelo admin e compartilhado com a família; nulo = sem acesso liberado.
        public string? CodigoResponsavel { get; set; }

        // Autorização de uso de imagem e voz (LGPD). null = não informado,
        // true = autoriza, false = não autoriza. Só se pode publicar a imagem
        // da criança quando for true.
        public bool? AutorizaImagem { get; set; }
        // Quando o responsável definiu a autorização pelo portal — a data do
        // consentimento/revogação, para demonstrar quando foi dado.
        public DateTime? AutorizaImagemEm { get; set; }

        public override bool Validate()
        {
            var validator = new AlunoValidator();
            var validation = validator.Validate(this);

            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                {
                    _errors.Add(error.ErrorMessage);
                }

                throw new DomainException("Alguns campos estão inválidos!", _errors);
            }

            return true;
        }
    }
}
