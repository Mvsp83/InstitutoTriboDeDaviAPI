using System;
using System.Collections.Generic;

namespace InstitutoTriboDeDavi.Application.DTO
{
    // Resultado da autenticação do responsável (código + nascimento). O
    // controller usa AlunoId/Nome para emitir o token e devolve o resumo.
    public class AcessoResponsavelDTO
    {
        public long AlunoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        // Faixa como número (o front mapeia para nome/cor, como no resto do app).
        public int Faixa { get; set; }
        public string Polo { get; set; } = string.Empty;
        public int Turma { get; set; }
    }

    // Painel só-leitura do responsável: dados do aluno + frequência, graduação,
    // avisos e calendário.
    public class PainelResponsavelDTO
    {
        public ResponsavelAlunoDTO Aluno { get; set; } = new();
        public FrequenciaResumoDTO Frequencia { get; set; } = new();
        public List<PresencaItemDTO> Presencas { get; set; } = new();
        public List<GraduacaoItemDTO> Graduacoes { get; set; } = new();
        public List<AvisoItemDTO> Avisos { get; set; } = new();
        public List<EventoItemDTO> Eventos { get; set; } = new();
    }

    public class ResponsavelAlunoDTO
    {
        public string Nome { get; set; } = string.Empty;
        public int Faixa { get; set; }
        public string Polo { get; set; } = string.Empty;
        public int Turma { get; set; }
        // Uso de imagem: null = não informado, true/false = decisão do responsável.
        public bool? AutorizaImagem { get; set; }
        public DateTime? AutorizaImagemEm { get; set; }
    }

    public class FrequenciaResumoDTO
    {
        public int TotalAulas { get; set; }
        public int Presencas { get; set; }
        public int Faltas { get; set; }
        public int Percentual { get; set; }
    }

    public class PresencaItemDTO
    {
        public long Id { get; set; }
        public DateTime Data { get; set; }
        public bool Presente { get; set; }
        // Justificativa da falta (preenchida pelo responsável). Null quando a
        // falta ainda não foi justificada ou quando o aluno esteve presente.
        public string Justificativa { get; set; }
        public DateTime? JustificadaEm { get; set; }
    }

    public class GraduacaoItemDTO
    {
        public DateTime Data { get; set; }
        public int FaixaAnterior { get; set; }
        public int FaixaNova { get; set; }
    }

    public class AvisoItemDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public DateTime Data { get; set; }
    }

    public class EventoItemDTO
    {
        public DateTime Data { get; set; }
        public DateTime? DataFim { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int Tipo { get; set; }
    }
}
