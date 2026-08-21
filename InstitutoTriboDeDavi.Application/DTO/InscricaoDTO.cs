using System;

namespace InstitutoTriboDeDavi.Application.DTO
{
    // Ficha enviada pelo site público. Chega sem autenticação, então o servidor
    // ignora qualquer campo de controle que venha no corpo (status, aluno,
    // revisão) e preenche esses valores por conta própria.
    public class InscricaoDTO
    {
        public long Id { get; set; }

        public long PoloId { get; set; }
        public int? Turma { get; set; }
        public bool JaEraAluno { get; set; }
        public int? TurmaAnterior { get; set; }

        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Rg { get; set; }
        public string Cpf { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Altura { get; set; }
        public int Faixa { get; set; }
        public string Escola { get; set; }
        public string Serie { get; set; }
        public string Periodo { get; set; }

        public int Parentesco { get; set; }
        public string ParentescoOutro { get; set; }
        public string NomeResponsavel { get; set; }
        public string RgResponsavel { get; set; }
        public string CpfResponsavel { get; set; }

        public string Rua { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string WhatsApp { get; set; }
        public string Telefone2 { get; set; }

        public string RespostasSaudeJson { get; set; }
        public string RespostasFamiliarJson { get; set; }
        public bool TemRestricaoMedica { get; set; }
        public string Medicamentos { get; set; }

        public bool AceitouTermo { get; set; }
        public bool AceitouImagem { get; set; }
        public bool AceitouComodato { get; set; }
        public bool AceitouLgpd { get; set; }
        public string NomeAssinatura { get; set; }
        public string VersaoTermos { get; set; }

        // Somente leitura (preenchidos pelo servidor).
        public int Ano { get; set; }
        public int Status { get; set; }
        public long? AlunoId { get; set; }
        public DateTime DataEnvio { get; set; }
        public DateTime? DataRevisao { get; set; }
        public string RevisadoPor { get; set; }
        public string ObservacaoRevisao { get; set; }
        // Preenchido nas listagens para a fila de revisão.
        public string PoloNome { get; set; }
    }

    // Decisão do revisor: aprova (podendo corrigir polo/turma) ou recusa.
    public class RevisaoInscricaoDTO
    {
        public long PoloId { get; set; }
        public int Turma { get; set; }
        public string Observacao { get; set; }
    }

    public class MatriculaDTO
    {
        public long Id { get; set; }
        public long AlunoId { get; set; }
        public int Ano { get; set; }
        public long PoloId { get; set; }
        public int Turma { get; set; }
        public long? InscricaoId { get; set; }
        public DateTime DataMatricula { get; set; }
        public bool Ativa { get; set; }
        public DateTime? DataEncerramento { get; set; }
        public string MotivoEncerramento { get; set; }
    }

    // Dado mínimo que o site público precisa para montar o formulário sem login.
    public class PoloPublicoDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; }
    }
}
