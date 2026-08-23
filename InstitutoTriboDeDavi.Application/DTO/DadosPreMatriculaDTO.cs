namespace InstitutoTriboDeDavi.Application.DTO
{
    // Dados de um aluno já cadastrado, devolvidos ao formulário público de
    // rematrícula para pré-preencher os campos (a família só edita o que mudou).
    // NÃO inclui saúde/pesquisa familiar/termos: esses são re-respondidos a cada
    // ano. Campos espelham o formulário (EnvioInscricao no front).
    public class DadosPreMatriculaDTO
    {
        public long AlunoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string DataNascimento { get; set; } = string.Empty; // yyyy-MM-dd
        public string Rg { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public double? Peso { get; set; }
        public double? Altura { get; set; }
        public int Faixa { get; set; }
        public string Escola { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;

        public int Parentesco { get; set; }
        public string NomeResponsavel { get; set; } = string.Empty;
        public string RgResponsavel { get; set; } = string.Empty;
        public string CpfResponsavel { get; set; } = string.Empty;

        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string WhatsApp { get; set; } = string.Empty;
        public string Telefone2 { get; set; } = string.Empty;

        // Polo/turma do último cadastro — sugestão que a família confirma/ajusta.
        public long PoloId { get; set; }
        public int TurmaAnterior { get; set; }
    }
}
