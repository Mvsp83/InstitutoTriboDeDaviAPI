using Dapper;
using ExcelDataReader;
using InstitutoTriboDeDavi.System.Factory.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace InstitutoTriboDeDavi.System.Factory
{
    public class FactoryPlanilhaDB : IFactoryPlanilhaDB
    {
        private readonly string _connectionString;

        public FactoryPlanilhaDB(string connectionString)
        {
            _connectionString = connectionString;
        }

        private DataTable ReadExcelFile(string filePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true 
                        }
                    });

                    DataTable table = result.Tables[0];
                    return table;
                }
            }
        }

        public void ImportDataAlunos(string filePath)
        {
            var dataTable = ReadExcelFile(filePath);


            using (var connection = new SqlConnection(""))
            {
                connection.Open();

                foreach (DataRow row in dataTable.Rows)
                {
                    string id = row["ID"].ToString();
                    string nome = row["NOME"].ToString();
                    string rg = row["RG"].ToString();
                    string cpf = row["CPF"].ToString();
                    string dataNasc = row["DATANASC"].ToString();
                    string peso = row["PESO"].ToString();
                    string faixa = row["FAIXA"].ToString();
                    string endereco = row["ENDERECO"].ToString();
                    string bairro = row["BAIRRO"].ToString();
                    string cidade = row["CIDADE"].ToString();
                    string celular = row["CELULAR"].ToString();
                    string responsavel = row["RESPONSAVEL"].ToString();
                    string parentesco = row["PARENTESCO"].ToString();
                    string rgResponsavel = row["RGRESPONSAVEL"].ToString();
                    string cpfResponsavel = row["CPFRESPONSAVEL"].ToString();
                    string escola = row["ESCOLA"].ToString();
                    string periodo = row["PERIODO"].ToString();
                    string poloId = row["POLOID"].ToString();

                    var existingRecord = connection.QueryFirstOrDefault<int>(
                        "SELECT COUNT(1) FROM ALUNOS WHERE NOME = @NOME", new { Nome = nome });

                    if (existingRecord > 0)
                    {
                        //var updateQuery = "UPDATE ALUNOS SET Nome = @Nome, Rg = @Rg, Cpf = @Cpf, DataNascimento = @DataNasc, Peso = @Peso, Faixa = @Faixa WHERE NOME = @NOME";
                        //connection.Execute(updateQuery, new { Nome = nome, RG = rg, CPF = cpf, DataNasc = dataNasc, Peso = peso, Faixa = faixa });
                    }
                    else
                    {
                        var insertQuery = "INSERT INTO ALUNOS (Nome, RG, CPF, DataNascimento, Peso, Faixa, Endereco, Bairro, Cidade, Celular," +
                                                             " Responsavel, Parentesco, RGResponsavel, CPFResponsavel, Escola, Periodo, PoloId) " +
                                                      "VALUES (@Nome, @RG, @CPF, @DataNasc, @Peso, @Faixa, @Endereco, @Bairro, @Cidade, @Celular," +
                                                             " @Responsavel, @Parentesco, @RGResponsavel, @CPFResponsavel, @Escola, @Periodo, @PoloId)";
                        connection.Execute(insertQuery, new { Nome = nome, RG = rg, CPF = cpf, DataNasc = dataNasc, Peso = peso, Faixa = faixa, 
                                                              Endereco = endereco, Bairro = bairro, Cidade = cidade, Celular = celular, Responsavel = responsavel,
                                                              Parentesco = parentesco, RGResponsavel = rgResponsavel, CPFResponsavel = cpfResponsavel,
                                                              Escola = escola, Periodo = periodo, PoloId = poloId});
                    }
                }
            }
        }

        public void ImportDataPolos(string filePath)
        {
            var dataTable = ReadExcelFile(filePath);


            using (var connection = new SqlConnection(""))
            {
                connection.Open();

                foreach (DataRow row in dataTable.Rows)
                {
                    //string id = row["ID"].ToString();
                    string nome = row["NOME"].ToString();
                    string informacoes = row["INFORMACOES"].ToString();
                    string endereco = row["ENDERECO"].ToString();
                    string bairro = row["BAIRRO"].ToString();
                    string cidade = row["CIDADE"].ToString();                  

                    var existingRecord = connection.QueryFirstOrDefault<int>(
                        "SELECT COUNT(1) FROM POLOS WHERE NOME = @NOME", new { Nome = nome });

                    if (existingRecord > 0)
                    {
                        //var updateQuery = "UPDATE ALUNOS SET Nome = @Nome, Rg = @Rg, Cpf = @Cpf, DataNascimento = @DataNasc, Peso = @Peso, Faixa = @Faixa WHERE NOME = @NOME";
                        //connection.Execute(updateQuery, new { Nome = nome, RG = rg, CPF = cpf, DataNasc = dataNasc, Peso = peso, Faixa = faixa });
                    }
                    else
                    {
                        var insertQuery = "INSERT INTO POLOS (Nome, Informacoes, Endereco, Bairro, Cidade)" +
                                                      "VALUES (@Nome, @Informacoes, @Endereco, @Bairro, @Cidade)";
                        
                        connection.Execute(insertQuery, new
                        {
                            Nome = nome,
                            Informacoes = informacoes, 
                            Endereco = endereco,
                            Bairro = bairro,
                            Cidade = cidade                           
                        });
                    }
                }
            }
        }
    }
}
