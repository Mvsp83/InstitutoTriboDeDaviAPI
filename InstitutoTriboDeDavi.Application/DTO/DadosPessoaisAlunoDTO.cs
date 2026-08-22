using System;
using System.Collections.Generic;

namespace InstitutoTriboDeDavi.Application.DTO
{
    // Pacote de portabilidade/acesso (LGPD art. 18): tudo que o sistema guarda
    // sobre um aluno, reunido para exportação. É uma "fotografia" legível — não
    // reaproveita os DTOs de escrita, para não vazar campos internos nem
    // depender do formato das telas.
    public class DadosPessoaisAlunoDTO
    {
        public DateTime GeradoEm { get; set; }
        public string GeradoPor { get; set; } = string.Empty;

        public DadosCadastraisDTO Cadastro { get; set; } = new();
        public List<MatriculaResumoDTO> Matriculas { get; set; } = new();
        public List<GraduacaoResumoDTO> Graduacoes { get; set; } = new();
        public List<PresencaResumoDTO> Presencas { get; set; } = new();
        public List<InscricaoResumoDTO> Inscricoes { get; set; } = new();
    }

    public class DadosCadastraisDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? RG { get; set; }
        public string? CPF { get; set; }
        public DateTime DataNascimento { get; set; }
        public double? Peso { get; set; }
        public double? Altura { get; set; }
        public string Faixa { get; set; } = string.Empty;
        public string? Endereco { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Celular { get; set; }
        public string? Telefone2 { get; set; }
        public string? Responsavel { get; set; }
        public string? Parentesco { get; set; }
        public string? RGResponsavel { get; set; }
        public string? CPFResponsavel { get; set; }
        public string? Escola { get; set; }
        public string? Serie { get; set; }
        public string? Periodo { get; set; }
        public long PoloId { get; set; }
        public int Turma { get; set; }
        public DateTime? AnonimizadoEm { get; set; }
    }

    public class MatriculaResumoDTO
    {
        public int Ano { get; set; }
        public long PoloId { get; set; }
        public int Turma { get; set; }
        public DateTime DataMatricula { get; set; }
        public bool Ativa { get; set; }
        public DateTime? DataEncerramento { get; set; }
        public string MotivoEncerramento { get; set; } = string.Empty;
    }

    public class GraduacaoResumoDTO
    {
        public DateTime Data { get; set; }
        public int FaixaAnterior { get; set; }
        public int FaixaNova { get; set; }
        public long PoloId { get; set; }
        public string Observacao { get; set; } = string.Empty;
        public string RegistradoPor { get; set; } = string.Empty;
    }

    public class PresencaResumoDTO
    {
        public DateTime Data { get; set; }
        public bool EstaPresente { get; set; }
        public long PoloId { get; set; }
        public string Observacoes { get; set; } = string.Empty;
    }

    public class InscricaoResumoDTO
    {
        public int Ano { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; }
        public bool AceitouTermo { get; set; }
        public bool AceitouImagem { get; set; }
        public bool AceitouComodato { get; set; }
        public bool AceitouLgpd { get; set; }
        public string VersaoTermos { get; set; } = string.Empty;
        public string NomeAssinatura { get; set; } = string.Empty;
    }

    // Candidato à eliminação por retenção: aluno sem atividade recente que já
    // passou do prazo definido na política de retenção.
    public class CandidatoRetencaoDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public long PoloId { get; set; }
        public DateTime? UltimaPresenca { get; set; }
        public int? UltimoAnoMatricula { get; set; }
        public int MesesInativo { get; set; }
    }
}
