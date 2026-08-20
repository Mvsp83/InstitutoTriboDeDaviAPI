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

        // Índices das colunas usados na identidade do aluno. O mapeamento é
        // posicional (frágil a mudanças no formulário); ValidarCabecalho ancora
        // a coluna de nome para detectar deslocamentos.
        private const int COL_NOME = 4;
        private const int COL_CPF = 7;

        // Respostas do formulário que significam "não informado" e devem virar
        // null (ex.: criança sem RG/CPF responde "Não tenho"). Comparado com o
        // valor inteiro da célula, sem acento/caixa.
        private static readonly HashSet<string> ValoresVazios = new()
        {
            "nao", "nao tenho", "nao possui", "n/a", "nenhum", "nenhuma", "-", "--"
        };

        // Datas do formulário vêm em pt-BR ("dd/MM/yyyy"). Parse sem cultura lia
        // 03/04 como 4 de março (MM/dd) e trocava aniversários silenciosamente.
        private static readonly string[] FormatosData =
            { "dd/MM/yyyy", "d/M/yyyy", "dd/MM/yy", "yyyy-MM-dd", "dd-MM-yyyy" };
        private static readonly CultureInfo CulturaBr = new("pt-BR");

        public FactoryPlanilhaDB(IAlunoRepository alunoRepository, IPoloRepository poloRepository)
        {
            _alunoRepository = alunoRepository;
            _poloRepository = poloRepository;
        }

        // Identifica o aluno já cadastrado priorizando o CPF (chave forte que
        // distingue homônimos); sem CPF, cai no nome. Antes a busca era só por
        // nome, então dois alunos de mesmo nome se sobrescreviam.
        private async Task<Aluno> BuscarAlunoExistenteAsync(string cpf, string nome)
        {
            if (!string.IsNullOrWhiteSpace(cpf))
            {
                var porCpf = await _alunoRepository.GetByCpf(cpf);
                if (porCpf != null)
                    return porCpf;
            }

            return await _alunoRepository.GetByNome(nome);
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

                    var alunoExistente = await BuscarAlunoExistenteAsync(LerString(row, COL_CPF), nome);

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
            return EhValorVazio(valor) ? null : valor;
        }

        private static DateTime? LerData(DataRow row, int coluna)
        {
            var valor = row[coluna];
            if (valor == null || valor == DBNull.Value) return null;
            if (valor is DateTime dt) return dt;
            return ParseData(valor.ToString());
        }

        // "Não", "Não tenho", "N/A", "-"... viram null (campo não informado).
        private static bool EhValorVazio(string valor)
        {
            var normal = RemoverAcentos(valor.Trim().ToLowerInvariant());
            return ValoresVazios.Contains(normal);
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

            if (rows == null || rows.Count == 0)
                return resultado;

            // Valida o cabeçalho antes de tocar nos dados: se a estrutura do
            // formulário mudou, aborta em vez de gravar campos desalinhados.
            var erroCabecalho = ValidarCabecalho(rows[0]);
            if (erroCabecalho != null)
            {
                resultado.Erros.Add(erroCabecalho);
                return resultado;
            }

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

                    var alunoExistente = await BuscarAlunoExistenteAsync(LerCelula(row, COL_CPF), nome);

                    if (alunoExistente != null)
                    {
                        // Só grava (e conta como Atualizado) quando algo mudou —
                        // evita reescrever registros idênticos a cada execução.
                        if (AplicarAtualizacao(alunoExistente, MapearAlunoDeSheets(row, poloId)))
                        {
                            await _alunoRepository.UpdateAsync(alunoExistente);
                            resultado.Atualizados++;
                        }
                        else
                        {
                            resultado.Ignorados++;
                        }
                    }
                    else
                    {
                        await _alunoRepository.CreateAsync(MapearAlunoDeSheets(row, poloId));
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

        // Monta um Aluno a partir de uma linha do Sheets (mesmo mapeamento usado
        // no insert e como "valores desejados" na atualização).
        private static Aluno MapearAlunoDeSheets(IList<object> row, long poloId)
        {
            var rua = LerCelula(row, 20);
            var numero = LerCelula(row, 21);
            var complemento = LerCelula(row, 22);

            return new Aluno
            {
                Nome = LerCelula(row, COL_NOME),
                RG = LerCelula(row, 6),
                CPF = LerCelula(row, COL_CPF),
                DataNascimento = ParseData(LerCelula(row, 5)) ?? DateTime.MinValue,
                Peso = ParseDouble(LerCelula(row, 9)),
                Faixa = FaixaMapper.Mapear(LerCelula(row, 11), LerCelula(row, 12)),
                Escola = LerCelula(row, 13),
                Periodo = LerCelula(row, 15),
                Parentesco = ParseParentesco(LerCelula(row, 16)),
                Responsavel = LerCelula(row, 17),
                RGResponsavel = LerCelula(row, 18),
                CPFResponsavel = LerCelula(row, 19),
                Endereco = MontarEndereco(rua, numero, complemento),
                Bairro = LerCelula(row, 23),
                Cidade = LerCelula(row, 24),
                Celular = LerCelula(row, 25),
                PoloId = poloId,
                Turma = ParseTurma(LerCelula(row, 3))
            };
        }

        // Copia os campos importáveis de origem→destino e devolve se algo mudou.
        // Nome não é alterado (identidade já resolvida); data só é sobrescrita se
        // veio válida; turma só é preenchida quando ainda pendente (0).
        private static bool AplicarAtualizacao(Aluno destino, Aluno origem)
        {
            var mudou = false;

            mudou |= AtualizarSeDiferente(destino.RG, origem.RG, v => destino.RG = v);
            mudou |= AtualizarSeDiferente(destino.CPF, origem.CPF, v => destino.CPF = v);
            mudou |= AtualizarSeDiferente(destino.Peso, origem.Peso, v => destino.Peso = v);
            mudou |= AtualizarSeDiferente(destino.Faixa, origem.Faixa, v => destino.Faixa = v);
            mudou |= AtualizarSeDiferente(destino.Escola, origem.Escola, v => destino.Escola = v);
            mudou |= AtualizarSeDiferente(destino.Periodo, origem.Periodo, v => destino.Periodo = v);
            mudou |= AtualizarSeDiferente(destino.Parentesco, origem.Parentesco, v => destino.Parentesco = v);
            mudou |= AtualizarSeDiferente(destino.Responsavel, origem.Responsavel, v => destino.Responsavel = v);
            mudou |= AtualizarSeDiferente(destino.RGResponsavel, origem.RGResponsavel, v => destino.RGResponsavel = v);
            mudou |= AtualizarSeDiferente(destino.CPFResponsavel, origem.CPFResponsavel, v => destino.CPFResponsavel = v);
            mudou |= AtualizarSeDiferente(destino.Endereco, origem.Endereco, v => destino.Endereco = v);
            mudou |= AtualizarSeDiferente(destino.Bairro, origem.Bairro, v => destino.Bairro = v);
            mudou |= AtualizarSeDiferente(destino.Cidade, origem.Cidade, v => destino.Cidade = v);
            mudou |= AtualizarSeDiferente(destino.Celular, origem.Celular, v => destino.Celular = v);
            mudou |= AtualizarSeDiferente(destino.PoloId, origem.PoloId, v => destino.PoloId = v);

            // Data só sobrescreve se for válida (não zera com MinValue/ilegível)
            if (origem.DataNascimento != DateTime.MinValue &&
                origem.DataNascimento != destino.DataNascimento)
            {
                destino.DataNascimento = origem.DataNascimento;
                mudou = true;
            }

            // Turma: só preenche quando pendente (o professor é quem atribui)
            if (destino.Turma == 0 && origem.Turma > 0)
            {
                destino.Turma = origem.Turma;
                mudou = true;
            }

            return mudou;
        }

        private static bool AtualizarSeDiferente<T>(T atual, T novo, Action<T> aplicar)
        {
            if (EqualityComparer<T>.Default.Equals(atual, novo))
                return false;

            aplicar(novo);
            return true;
        }

        // Detecta deslocamento de colunas ancorando na coluna de nome: se ela
        // não parecer o cabeçalho de "Nome"/"Aluno", a estrutura mudou. Retorna
        // a mensagem de erro ou null quando o cabeçalho está coerente.
        private static string ValidarCabecalho(IList<object> cabecalho)
        {
            var nomeHeader = COL_NOME < cabecalho.Count
                ? cabecalho[COL_NOME]?.ToString()?.Trim()
                : null;

            var normalizado = RemoverAcentos((nomeHeader ?? string.Empty).ToLowerInvariant());
            if (normalizado.Contains("nome") || normalizado.Contains("aluno"))
                return null;

            var lido = string.Join(" | ", cabecalho.Select(c => c?.ToString()));
            return $"Cabeçalho inesperado: a coluna de nome (posição {COL_NOME + 1}/E) não parece ser " +
                   $"'Nome' (lido: \"{nomeHeader}\"). O formulário do Google pode ter mudado de estrutura — " +
                   $"importação abortada para não desalinhar os dados. Cabeçalho completo: [{lido}]";
        }

        private static string RemoverAcentos(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto ?? string.Empty;

            var decomposto = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(decomposto.Length);
            foreach (var c in decomposto)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        // Helper específico para listas do Sheets (IList<object>)
        private static string LerCelula(IList<object> row, int coluna)
        {
            if (coluna >= row.Count) return null;
            var valor = row[coluna]?.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(valor)) return null;
            return EhValorVazio(valor) ? null : valor;
        }

        // Parse tolerante e em pt-BR: tenta os formatos comuns e, por fim, o
        // parser da cultura brasileira. Evita a ambiguidade dd/MM x MM/dd.
        private static DateTime? ParseData(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            valor = valor.Trim();

            if (DateTime.TryParseExact(valor, FormatosData, CulturaBr,
                    DateTimeStyles.None, out var exato))
                return exato;

            if (DateTime.TryParse(valor, CulturaBr, DateTimeStyles.None, out var br))
                return br;

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
