using System;
using System.Linq;
using InstitutoTriboDeDavi.Domain.Common;
using InstitutoTriboDeDavi.Domain.Exceptions;

namespace InstitutoTriboDeDavi.Domain.Entities
{
    public enum StatusInscricao
    {
        Pendente = 0,
        Aprovada = 1,
        Recusada = 2,
    }

    // Ficha de inscrição enviada pelo responsável no site público. É a
    // submissão como veio: guardamos o que a família preencheu, inclusive os
    // aceites dos termos, e só depois da revisão isso vira Aluno + Matrícula.
    //
    // O questionário de aptidão física (PAR-Q, Lei 16.331/2014) e a pesquisa
    // familiar ficam em JSON porque as perguntas mudam de um ano para outro —
    // guardar como enviado preserva a resposta original. Os poucos campos que
    // o professor precisa ver rápido (restrição médica, medicamentos) são
    // colunas próprias.
    public class Inscricao : Base
    {
        // ── Destino ───────────────────────────────────────────────────────
        public int Ano { get; set; }
        public long PoloId { get; set; }
        public int? Turma { get; set; }
        public bool JaEraAluno { get; set; }
        public int? TurmaAnterior { get; set; }

        // ── Dados do menor ────────────────────────────────────────────────
        public string Nome { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string Rg { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public decimal? Peso { get; set; }
        public decimal? Altura { get; set; }
        public int Faixa { get; set; }
        public string Escola { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;

        // ── Responsável ───────────────────────────────────────────────────
        public int Parentesco { get; set; }
        public string ParentescoOutro { get; set; } = string.Empty;
        public string NomeResponsavel { get; set; } = string.Empty;
        public string RgResponsavel { get; set; } = string.Empty;
        public string CpfResponsavel { get; set; } = string.Empty;

        // ── Endereço e contato ────────────────────────────────────────────
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string WhatsApp { get; set; } = string.Empty;
        public string Telefone2 { get; set; } = string.Empty;

        // ── Questionários (JSON) ──────────────────────────────────────────
        public string RespostasSaudeJson { get; set; } = string.Empty;
        public string RespostasFamiliarJson { get; set; } = string.Empty;
        // Destaques de saúde, para o professor ver sem abrir a ficha inteira.
        public bool TemRestricaoMedica { get; set; }
        public string Medicamentos { get; set; } = string.Empty;

        // ── Aceites ───────────────────────────────────────────────────────
        // Guardados como evidência: quem aceitou, quando e qual versão do texto.
        public bool AceitouTermo { get; set; }
        public bool AceitouImagem { get; set; }
        public bool AceitouComodato { get; set; }
        public bool AceitouLgpd { get; set; }
        public string NomeAssinatura { get; set; } = string.Empty;
        public string VersaoTermos { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; }

        // Código de acesso ao portal, gerado quando a família envia a ficha. É
        // repassado ao Aluno na aprovação — assim a família usa o mesmo código
        // desde a inscrição.
        public string CodigoResponsavel { get; set; } = string.Empty;

        // ── Revisão ───────────────────────────────────────────────────────
        public int Status { get; set; } = (int)StatusInscricao.Pendente;
        public long? AlunoId { get; set; }
        public DateTime? DataRevisao { get; set; }
        public string RevisadoPor { get; set; } = string.Empty;
        public string ObservacaoRevisao { get; set; } = string.Empty;

        public override bool Validate()
        {
            _errors.Clear();

            if (string.IsNullOrWhiteSpace(Nome))
                _errors.Add("O nome do aluno é obrigatório.");
            if (DataNascimento == default)
                _errors.Add("A data de nascimento é obrigatória.");
            if (DataNascimento > DateTime.Today)
                _errors.Add("A data de nascimento não pode estar no futuro.");
            if (PoloId <= 0)
                _errors.Add("Selecione o polo.");
            if (string.IsNullOrWhiteSpace(NomeResponsavel))
                _errors.Add("O nome do responsável é obrigatório.");
            if (string.IsNullOrWhiteSpace(WhatsApp))
                _errors.Add("O WhatsApp do responsável é obrigatório.");

            // Sem os aceites não há base para tratar os dados nem para o menor
            // participar — a inscrição não pode ser registrada.
            if (!AceitouTermo)
                _errors.Add("É preciso aceitar o termo de participação.");
            if (!AceitouLgpd)
                _errors.Add("É preciso autorizar o tratamento dos dados.");
            if (string.IsNullOrWhiteSpace(NomeAssinatura))
                _errors.Add("Informe o nome completo do responsável na assinatura.");

            if (_errors.Any())
                throw new DomainException("Alguns campos estão inválidos!", _errors);

            return true;
        }
    }
}
