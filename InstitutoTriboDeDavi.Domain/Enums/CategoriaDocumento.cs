using System.ComponentModel;

namespace InstitutoTriboDeDavi.Domain.Enums
{
    // Categorias de documentos armazenados no Google Drive.
    // O Description é o nome da subpasta criada dentro da pasta raiz.
    public enum CategoriaDocumento
    {
        [Description("DRE")]
        Dre,
        [Description("Balanço")]
        Balanco,
        [Description("Relatório de Atividades")]
        RelatorioAtividades,
        [Description("Modelos de Documentos")]
        Modelos
    }
}
