using System.ComponentModel;

namespace InstitutoTriboDeDavi.Domain.Enums
{
    // Categorias e estado de conservação dos bens do patrimônio. O valor int é o
    // que trafega no DTO; o portal React tem seus próprios rótulos.
    public enum CategoriaBem
    {
        [Description("Quimono")] Quimono,
        [Description("Faixa")] Faixa,
        [Description("Tatame")] Tatame,
        [Description("Veículo")] Veiculo,
        [Description("Imóvel")] Imovel,
        [Description("Equipamento")] Equipamento,
        [Description("Móvel")] Movel,
        [Description("Eletrônico")] Eletronico,
        [Description("Outro")] Outro
    }

    public enum EstadoConservacao
    {
        [Description("Novo")] Novo,
        [Description("Bom")] Bom,
        [Description("Regular")] Regular,
        [Description("Ruim")] Ruim,
        [Description("Baixado")] Baixado
    }
}
