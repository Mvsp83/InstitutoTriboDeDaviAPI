using ExcelDataReader;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Enums;
using InstitutoTriboDeDavi.Application.Import;
using System.Data;
using System.Globalization;
using System.Text;

namespace InstitutoTriboDeDavi.Infrastructure.Import
{
    public class FactoryPlanilhaDB : IFactoryPlanilhaDB
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly IPoloRepository _poloRepository;

        public FactoryPlanilhaDB(IAlunoRepository alunoRepository, IPoloRepository poloRepository)
        {
            _alunoRepository = alunoRepository;
            _poloRepository = poloRepository;
        }

        public async Task<ImportacaoResultado> ImportarAlunosAsync(Stream arquivo, long poloIdPadrao)
        {
            var resultado = new ImportacaoResultado();
            var tabela = LerExcel(arquivo);

            foreach (DataRow row in tabela.Rows)
            {
                try
                {
                    var nome = LerString(row, 4);

                    if (string.IsNullOrWhiteSpace(nome))
                    {
                        resultado.Ignorados++;
                        continue;
                    }

                    // Resolver PoloId pelo nome do polo na planilha
                    var poloNome = LerString(row, 1);
                    var poloId = await ResolverPoloIdAsync(poloNome, poloIdPadrao);

                    // Turma: "Turma 1" → 1, "Turma 2" → 2, novo aluno → 0 (pendente)
                    var turmaTexto = LerString(row, 3);
                    var turma = ParseTurma(turmaTexto);

                    // Endereço composto
                    var rua = LerString(row, 20);
                    var numero = LerString(row, 21);
                    var complemento = LerString(row, 22);
                    var endereco = MontarEndereco(rua, numero, complemento);

                    var alunoExistente = await _alunoRepository.GetByNome(nome);

                    if (alunoExistente != null)
                    {
                        // Atualiza dados do aluno existente
                        alunoExistente.RG = LerString(row, 6);
                        alunoExistente.CPF = LerString(row, 7);
                        alunoExistente.DataNascimento = LerData(row, 5) ?? alunoExistente.DataNascimento;
                        alunoExistente.Peso = LerDouble(row, 9);
                        alunoExistente.Faixa = FaixaMapper.Mapear(LerString(row, 11), LerString(row, 12));
                        alunoExistente.Escola = LerString(row, 13);
                        alunoExistente.Periodo = LerString(row, 15);
                        alunoExistente.Parentesco = ParseParentesco(LerString(row, 16));
                        alunoExistente.Responsavel = LerString(row, 17);
                        alunoExistente.RGResponsavel = LerString(row, 18);
                        alunoExistente.CPFResponsavel = LerString(row, 19);
                        alunoExistente.Endereco = endereco;
                        alunoExistente.Bairro = LerString(row, 23);
                        alunoExistente.Cidade = LerString(row, 24);
                        alunoExistente.Celular = LerString(row, 25);
                        alunoExistente.PoloId = poloId;

                        // Só atualiza turma se estava pendente (0) e veio preenchida
                        if (alunoExistente.Turma == 0 && turma > 0)
                            alunoExistente.Turma = turma;

                        await _alunoRepository.UpdateAsync(alunoExistente);
                        resultado.Atualizados++;
                    }
                    else
                    {
                        var novoAluno = new Aluno
                        {
                            Nome = nome,
                            RG = LerString(row, 6),
                            CPF = LerString(row, 7),
                            DataNascimento = LerData(row, 5) ?? DateTime.MinValue,
                            Peso = LerDouble(row, 9),
                            Faixa = FaixaMapper.Mapear(LerString(row, 11), LerString(row, 12)),
                            Escola = LerString(row, 13),
                            Periodo = LerString(row, 15),
                            Parentesco = ParseParentesco(LerString(row, 16)),
                            Responsavel = LerString(row, 17),
                            RGResponsavel = LerString(row, 18),
                            CPFResponsavel = LerString(row, 19),
                            Endereco = endereco,
                            Bairro = LerString(row, 23),
                            Cidade = LerString(row, 24),
                            Celular = LerString(row, 25),
                            PoloId = poloId,
                            Turma = turma  // 0 = pendente, será atribuída pelo professor
                        };

                        await _alunoRepository.CreateAsync(novoAluno);
                        resultado.Inseridos++;
                    }
                }
                catch (Exception ex)
                {
                    resultado.Erros.Add($"Linha com nome '{LerString(row, 4)}': {ex.Message}");
                }
            }

            return resultado;
        }

        // --- Helpers ---

        private DataTable LerExcel(Stream stream)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using var reader = ExcelReaderFactory.CreateReader(stream);
            var dataset = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
            });

            return dataset.Tables[0];
        }

        private async Task<long> ResolverPoloIdAsync(string poloNome, long poloIdPadrao)
        {
            if (string.IsNullOrWhiteSpace(poloNome))
                return poloIdPadrao;

            var polo = await _poloRepository.GetByNome(poloNome.Trim());
            return polo?.Id ?? poloIdPadrao;
        }

        private static string LerString(DataRow row, int coluna)
        {
            var valor = row[coluna]?.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(valor)) return null;
            if (valor.Equals("NÃO", StringComparison.OrdinalIgnoreCase)) return null;
            if (valor.Equals("Não", StringComparison.OrdinalIgnoreCase)) return null;
            return valor;
        }

        private static DateTime? LerData(DataRow row, int coluna)
        {
            var valor = row[coluna];
            if (valor == null || valor == DBNull.Value) return null;
            if (valor is DateTime dt) return dt;
            if (DateTime.TryParse(valor.ToString(), out var parsed)) return parsed;
            return null;
        }

        private static double? LerDouble(DataRow row, int coluna)
        {
            var valor = row[coluna]?.ToString()?.Replace(",", ".");
            if (double.TryParse(valor, NumberStyles.Any,
                CultureInfo.InvariantCulture, out var result))
                return result;
            return null;
        }

        private static int ParseTurma(string turmaTexto)
        {
            if (string.IsNullOrWhiteSpace(turmaTexto)) return 0;
            var numero = turmaTexto.Replace("Turma", "").Trim();
            return int.TryParse(numero, out var t) ? t : 0;
        }

        private static string MontarEndereco(string rua, string numero, string complemento)
        {
            var partes = new[] { rua, numero, complemento }
                .Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(", ", partes);
        }

        private static Parentesco? ParseParentesco(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return null;

            return texto.ToLower() switch
            {
                "pai" => Parentesco.Pai,
                "mãe" or "mae" => Parentesco.Mae,
                "tio" => Parentesco.Tio,
                "tia" => Parentesco.Tia,
                "avô" or "avó" or "avo" => Parentesco.Avo,
                _ => Parentesco.Outros
            };
        }

        public async Task<ImportacaoResultado> ImportarAlunosDeSheetsAsync(IList<IList<object>> rows, long poloId)
        {
            var resultado = new ImportacaoResultado();

            // Pula a linha de cabeçalho
            foreach (var row in rows.Skip(1))
            {
                try
                {
                    var nome = LerCelula(row, 4);

                    if (string.IsNullOrWhiteSpace(nome))
                    {
                        resultado.Ignorados++;
                        continue;
                    }

                    var turmaTexto = LerCelula(row, 3);
                    var turma = ParseTurma(turmaTexto);

                    var rua = LerCelula(row, 20);
                    var numero = LerCelula(row, 21);
                    var complemento = LerCelula(row, 22);
                    var endereco = MontarEndereco(rua, numero, complemento);

                    var alunoExistente = await _alunoRepository.GetByNome(nome);

                    if (alunoExistente != null)
                    {
                        alunoExistente.RG = LerCelula(row, 6);
                        alunoExistente.CPF = LerCelula(row, 7);
                        alunoExistente.DataNascimento = ParseData(LerCelula(row, 5)) ?? alunoExistente.DataNascimento;
                        alunoExistente.Peso = ParseDouble(LerCelula(row, 9));
                        alunoExistente.Faixa = FaixaMapper.Mapear(LerCelula(row, 11), LerCelula(row, 12));
                        alunoExistente.Escola = LerCelula(row, 13);
                        alunoExistente.Periodo = LerCelula(row, 15);
                        alunoExistente.Parentesco = ParseParentesco(LerCelula(row, 16));
                        alunoExistente.Responsavel = LerCelula(row, 17);
                        alunoExistente.RGResponsavel = LerCelula(row, 18);
                        alunoExistente.CPFResponsavel = LerCelula(row, 19);
                        alunoExistente.Endereco = endereco;
                        alunoExistente.Bairro = LerCelula(row, 23);
                        alunoExistente.Cidade = LerCelula(row, 24);
                        alunoExistente.Celular = LerCelula(row, 25);
                        alunoExistente.PoloId = poloId;

                        if (alunoExistente.Turma == 0 && turma > 0)
                            alunoExistente.Turma = turma;

                        await _alunoRepository.UpdateAsync(alunoExistente);
                        resultado.Atualizados++;
                    }
                    else
                    {
                        var novoAluno = new Aluno
                        {
                            Nome = nome,
                            RG = LerCelula(row, 6),
                            CPF = LerCelula(row, 7),
                            DataNascimento = ParseData(LerCelula(row, 5)) ?? DateTime.MinValue,
                            Peso = ParseDouble(LerCelula(row, 9)),
                            Faixa = FaixaMapper.Mapear(LerCelula(row, 11), LerCelula(row, 12)),
                            Escola = LerCelula(row, 13),
                            Periodo = LerCelula(row, 15),
                            Parentesco = ParseParentesco(LerCelula(row, 16)),
                            Responsavel = LerCelula(row, 17),
                            RGResponsavel = LerCelula(row, 18),
                            CPFResponsavel = LerCelula(row, 19),
                            Endereco = endereco,
                            Bairro = LerCelula(row, 23),
                            Cidade = LerCelula(row, 24),
                            Celular = LerCelula(row, 25),
                            PoloId = poloId,
                            Turma = turma
                        };

                        await _alunoRepository.CreateAsync(novoAluno);
                        resultado.Inseridos++;
                    }
                }
                catch (Exception ex)
                {
                    resultado.Erros.Add($"Linha com nome '{LerCelula(row, 4)}': {ex.Message}");
                }
            }

            return resultado;
        }

        // Helper específico para listas do Sheets (IList<object>)
        private static string LerCelula(IList<object> row, int coluna)
        {
            if (coluna >= row.Count) return null;
            var valor = row[coluna]?.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(valor)) return null;
            if (valor.Equals("NÃO", StringComparison.OrdinalIgnoreCase)) return null;
            if (valor.Equals("Não", StringComparison.OrdinalIgnoreCase)) return null;
            return valor;
        }

        private static DateTime? ParseData(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            if (DateTime.TryParse(valor, out var dt)) return dt;
            return null;
        }

        private static double? ParseDouble(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            var normalizado = valor.Replace(",", ".");
            if (double.TryParse(normalizado, NumberStyles.Any,
                CultureInfo.InvariantCulture, out var result))
                return result;
            return null;
        }
    }
}
