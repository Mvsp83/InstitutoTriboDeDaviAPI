using System;

namespace InstitutoTriboDeDavi.Application.DTO
{
    public class GraduacaoDTO
    {
        public long Id { get; set; }
        public long AlunoId { get; set; }
        public long PoloId { get; set; }
        public int FaixaAnterior { get; set; }
        public int FaixaNova { get; set; }
        public DateTime Data { get; set; }
        public string Observacao { get; set; }
        public string RegistradoPor { get; set; }
        // Preenchidos na listagem, para a tela não precisar cruzar com alunos.
        public string NomeAluno { get; set; }
        public string PoloNome { get; set; }
    }

    // Uma graduação de turma: vários alunos promovidos na mesma data.
    public class GraduacaoLoteDTO
    {
        public DateTime Data { get; set; }
        public string Observacao { get; set; }
        public ItemGraduacaoDTO[] Alunos { get; set; }
    }

    public class ItemGraduacaoDTO
    {
        public long AlunoId { get; set; }
        public int FaixaNova { get; set; }
    }

    public class ResultadoGraduacaoDTO
    {
        public int Graduados { get; set; }
        // Alunos que não puderam ser graduados, com o motivo.
        public string[] Ignorados { get; set; }
        public string Mensagem { get; set; }
    }
}
