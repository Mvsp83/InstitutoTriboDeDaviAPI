using InstitutoTriboDeDavi.Domain.Common;

namespace InstitutoTriboDeDavi.Domain.Entities.Business
{
    // Arquivo de documento oficial guardado no próprio banco (sem dependência do
    // Google Drive). O conteúdo binário fica em Conteudo (varbinary(max)); a
    // listagem lê só os metadados, nunca o binário.
    public class DocumentoArquivo : Base
    {
        // Categoria (DRE, Balanço, Relatório de Atividades, Modelos) — int do
        // enum CategoriaDocumento.
        public int Categoria { get; set; }
        public string Nome { get; set; }
        public string ContentType { get; set; }
        public long TamanhoBytes { get; set; }
        public byte[] Conteudo { get; set; }
        public System.DateTime DataCriacao { get; set; }

        public override bool Validate() => true;
    }
}
