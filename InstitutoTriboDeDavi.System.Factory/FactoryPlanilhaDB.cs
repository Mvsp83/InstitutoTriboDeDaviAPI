//using Dapper;
//using ExcelDataReader;
//using Microsoft.Extensions.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.Text;

//namespace InstitutoTriboDeDavi.System.Factory
//{
//    public class FactoryPlanilhaDB : IFactoryPlanilhaDB
//    {
//        private readonly string _connectionString;

//        public FactoryPlanilhaDB(string connectionString)
//        {
//            _connectionString = connectionString;
//        }

//        //private readonly string _connectionString;

//        //public FactoryPlanilhaDB(IConfiguration configuration)
//        //{
//        //    _connectionString = configuration.GetConnectionString("DefaultConnection");
//        //}

//        // Função para ler o arquivo Excel e retornar os dados como DataTable
//        //private DataTable ReadExcelFile(string filePath)
//        //{
//        //    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
//        //    {
//        //        // Determina o formato do arquivo Excel (xls ou xlsx)
//        //        //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
//        //        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

//        //        using (var reader = ExcelReaderFactory.CreateReader(stream))
//        //        {
//        //            var result = reader.AsDataSet();
//        //            // Assume que os dados estão na primeira planilha
//        //            return result.Tables[0];
//        //        }
//        //    }
//        //}

//        //private DataTable ReadExcelFile(string filePath)
//        //{
//        //    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

//        //    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
//        //    {
//        //        //using (var reader = ExcelReaderFactory.CreateReader(stream))
//        //        //{
//        //        //    var result = reader.AsDataSet();
//        //        //    DataTable table = result.Tables[0]; // Assume que a primeira tabela é a relevante

//        //        //    // Listar as colunas da tabela para diagnóstico
//        //        //    foreach (DataColumn column in table.Columns)
//        //        //    {
//        //        //        Console.WriteLine($"Column Name: {column.ColumnName}");
//        //        //    }

//        //        //    return table;
//        //        //}

//        //        using (var reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
//        //        {
//        //            FallbackEncoding = Encoding.UTF8, // Para garantir a codificação correta
//        //                                              // Configuração para usar a primeira linha como cabeçalho
//        //            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
//        //            {
//        //                UseHeaderRow = true
//        //            }
//        //        }))
//        //        {
//        //            var result = reader.AsDataSet();
//        //            DataTable table = result.Tables[0];
//        //            return table;
//        //        }
//        //    }
//        //}

//        private DataTable ReadExcelFile(string filePath)
//        {
//            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

//            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
//            {
//                using (var reader = ExcelReaderFactory.CreateReader(stream))
//                {
//                    // Configurar o DataSet para usar a primeira linha como cabeçalho
//                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
//                    {
//                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
//                        {
//                            UseHeaderRow = true // Usar a primeira linha como cabeçalho
//                        }
//                    });

//                    DataTable table = result.Tables[0]; // Selecionar a primeira tabela da planilha
//                    return table;
//                }
//            }
//        }



//        // Função que executa a importação e atualização no banco de dados
//        public void ImportData(string filePath)
//        {
//            var dataTable = ReadExcelFile(filePath);


//            using (var connection = new SqlConnection(""))
//            {
//                connection.Open();

//                foreach (DataRow row in dataTable.Rows)
//                {
//                    string id = row["ID"].ToString();
//                    string nome = row["Nome"].ToString(); // Exemplo: coluna 'Nome'
//                    string cpf = row["CPF"].ToString();
//                    string dataNasc = row["DataNasc"].ToString();
//                    string faixa = row["Faixa"].ToString(); // Exemplo: coluna 'Email'                  
                    

//                    // Verifica se o registro com o mesmo CPF já existe no banco
//                    var existingRecord = connection.QueryFirstOrDefault<int>(
//                        "SELECT COUNT(1) FROM ALUNOS WHERE CPF = @CPF", new { CPF = cpf });

//                    if (existingRecord > 0)
//                    {
//                        // Se já existe, realiza a atualização
//                        var updateQuery = "UPDATE ALUNOS SET Nome = @Nome, DataNasc = @DataNasc, Faixa = @Faixa WHERE CPF = @CPF";
//                        connection.Execute(updateQuery, new { Nome = nome, CPF = cpf, DataNasc = dataNasc, Faixa = faixa });
//                    }
//                    else
//                    {
//                        // Se não existe, realiza a inserção
//                        var insertQuery = "INSERT INTO ALUNOS (Nome, CPF, DataNasc, Faixa) VALUES (@Nome, @CPF, @DataNasc, @Faixa)";
//                        connection.Execute(insertQuery, new { Nome = nome, CPF = cpf, DataNasc = dataNasc, Faixa = faixa });
//                    }
//                }
//            }
//        }
//    }
//}
