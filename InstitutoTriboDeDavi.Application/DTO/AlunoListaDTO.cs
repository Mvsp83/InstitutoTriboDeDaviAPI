using InstitutoTriboDeDavi.Domain.Enums;

namespace InstitutoTriboDeDavi.Application.DTO
{
    // DTO enxuto para a LISTAGEM de alunos: só os campos que a lista, os selos
    // e a ordenação usam. Não carrega dados pessoais desnecessários (CPF, RG,
    // endereço, telefones, dados do responsável) — esses só saem na ficha/edição
    // (GET por id) e na exportação LGPD. Reduz payload/custo e exposição de PII.
    public class AlunoListaDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public Faixa Faixa { get; set; }
        public long PoloId { get; set; }
        public int Turma { get; set; }
        public bool TemFoto { get; set; }
        public bool? AutorizaImagem { get; set; }
        public DateTime DataNascimento { get; set; }
        public bool EhAdulto { get; set; }
        // Nome do responsável — usado só na impressão de "sem autorização de
        // imagem". Não é dado sensível como CPF/RG e evita uma busca por aluno.
        public string Responsavel { get; set; }
    }
}
