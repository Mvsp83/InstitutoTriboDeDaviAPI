using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Infrastructure.Import;

namespace InstitutoTriboDeDavi.Tests
{
    public class FaixaMapperTests
    {
        [Theory]
        [InlineData("Branca", "1", Faixa.Branca1)]
        [InlineData("branca", null, Faixa.Branca)]
        [InlineData("Iniciante", null, Faixa.Branca)]
        [InlineData("Cinza", "2", Faixa.Cinza2)]
        [InlineData("Amarela", "4", Faixa.Amarela4)]
        [InlineData("Laranja", "0", Faixa.Laranja)]
        [InlineData("Verde", "3", Faixa.Verde3)]
        [InlineData("Azul", "2.0", Faixa.Azul2)] // Sheets manda número como "2.0"
        [InlineData("Roxa", "1", Faixa.Roxa1)]
        [InlineData("Marrom", "4", Faixa.Marrom4)]
        [InlineData("Preta", "3", Faixa.Preta)]  // preta não tem graus no mapper
        [InlineData(null, null, Faixa.Branca)]    // vazio → iniciante
        [InlineData("qualquer coisa", null, Faixa.Branca)]
        public void Mapear_TextoDoFormulario_RetornaFaixaEsperada(string? faixa, string? graus, Faixa esperada)
        {
            Assert.Equal(esperada, FaixaMapper.Mapear(faixa!, graus!));
        }

        [Theory]
        [InlineData("Amarela", Faixa.Amarela)] // com acento normalizado: "amárela" não ocorre; testa acento real
        [InlineData("cinza ", Faixa.Cinza)]    // espaço extra
        public void Mapear_ToleraVariacoesDeTexto(string faixa, Faixa esperada)
        {
            Assert.Equal(esperada, FaixaMapper.Mapear(faixa, ""));
        }

        // Regressão do bug do prefixo: "Faixa Azul" era normalizado para
        // "faixa" (primeira palavra) e caía no default Branca — rebaixando
        // o aluno a cada sincronização noturna.
        [Theory]
        [InlineData("Faixa Azul", "2", Faixa.Azul2)]
        [InlineData("faixa branca", "1", Faixa.Branca1)]
        [InlineData("FAIXA VERDE", null, Faixa.Verde)]
        [InlineData("Faixa cinza e branca", null, Faixa.Cinza)] // prefixo + composta
        [InlineData("Cinza e branca", null, Faixa.Cinza)]       // composta sem prefixo
        public void Mapear_ToleraPrefixoFaixaEFaixasCompostas(string faixa, string? graus, Faixa esperada)
        {
            Assert.Equal(esperada, FaixaMapper.Mapear(faixa, graus!));
        }
    }
}
