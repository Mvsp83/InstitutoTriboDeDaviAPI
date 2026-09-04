using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using InstitutoTriboDeDavi.Application.Repositories;
using InstitutoTriboDeDavi.Domain.Entities;
using InstitutoTriboDeDavi.Domain.Entities.Business;
using InstitutoTriboDeDavi.Application.Import;
using InstitutoTriboDeDavi.Infrastructure.Configuration;
using InstitutoTriboDeDavi.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace InstitutoTriboDeDavi.Infrastructure.GoogleSheets
{
    public class GoogleSheetsService : IGoogleSheetsService
    {
        private readonly GoogleSheetsConfig _config;
        private readonly IFactoryPlanilhaDB _factory;
        private readonly ISincronizacaoHistoricoRepository _historicoRepository;
        private readonly ILogger<GoogleSheetsService> _logger;

        public GoogleSheetsService(
            IOptions<GoogleSheetsConfig> config,
            IFactoryPlanilhaDB factory,
            ISincronizacaoHistoricoRepository historicoRepository,
            ILogger<GoogleSheetsService> logger)
        {
            _config = config.Value;
            _factory = factory;
            _historicoRepository = historicoRepository;
            _logger = logger;
        }

        public async Task<List<ImportacaoResultado>> SincronizarTodasAsPlanilhasAsync(string origem = "Automatico")
        {
            var resultados = new List<ImportacaoResultado>();

            foreach (var planilha in _config.Planilhas)
            {
                try
                {
                    _logger.LogInformation("Sincronizando planilha do polo {PoloNome}...", planilha.PoloNome);
                    var resultado = await SincronizarPlanilhaAsync(planilha.PoloId, origem);
                    resultados.Add(resultado);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao sincronizar planilha do polo {PoloNome}", planilha.PoloNome);
                    resultados.Add(new ImportacaoResultado
                    {
                        PoloNome = planilha.PoloNome,
                        Erros = new List<string> { $"Falha geral: {ex.Message}" }
                    });
                }
            }

            return resultados;
        }

        public async Task<ImportacaoResultado> SincronizarPlanilhaAsync(long poloId, string origem = "Manual")
        {
            var planilha = _config.Planilhas.FirstOrDefault(p => p.PoloId == poloId)
                ?? throw new Domain.Exceptions.DomainException($"Polo com ID {poloId} não encontrado na configuração.");

            var sheetsService = CriarSheetsService();
            var range = $"{planilha.NomeAba}!A:Z";
            var request = sheetsService.Spreadsheets.Values.Get(planilha.SpreadsheetId, range);
            var response = await request.ExecuteAsync();
            var rows = response.Values;

            ImportacaoResultado resultado;

            if (rows == null || rows.Count <= 1)
            {
                resultado = new ImportacaoResultado
                {
                    PoloNome = planilha.PoloNome,
                    Ignorados = 0,
                    Inseridos = 0,
                    Atualizados = 0
                };
            }
            else
            {
                resultado = await _factory.ImportarAlunosDeSheetsAsync(rows, planilha.PoloId);
                resultado.PoloNome = planilha.PoloNome;
            }

            // Salvar histórico
            await _historicoRepository.CreateAsync(new SincronizacaoHistorico
            {
                DataExecucao = FusoBrasil.Agora, // horário de Brasília mesmo em servidor UTC
                PoloId = planilha.PoloId,
                PoloNome = planilha.PoloNome,
                Inseridos = resultado.Inseridos,
                Atualizados = resultado.Atualizados,
                Ignorados = resultado.Ignorados,
                Sucesso = !resultado.Erros.Any(),
                Erros = resultado.Erros.Any()
                    ? JsonSerializer.Serialize(resultado.Erros)
                    : null,
                Origem = origem
            });

            _logger.LogInformation(
                "Polo {PoloNome}: {Inseridos} inseridos, {Atualizados} atualizados, {Ignorados} ignorados. Origem: {Origem}",
                planilha.PoloNome, resultado.Inseridos, resultado.Atualizados, resultado.Ignorados, origem);

            return resultado;
        }

        private SheetsService CriarSheetsService()
        {
            var configurado = _config.CredenciaisJson ?? string.Empty;

            // Aceita duas formas: o CONTE\u00DADO do JSON (ideal para nuvem \u2014 vem de
            // uma vari\u00E1vel de ambiente) ou um CAMINHO de arquivo (dev local).
            // Distingue pelo primeiro caractere n\u00E3o-branco: '{' => \u00E9 o JSON.
            string json;
            if (configurado.TrimStart('\uFEFF', ' ', '\t', '\r', '\n').StartsWith("{"))
            {
                json = configurado;
            }
            else
            {
                var credenciaisPath = Path.Combine(AppContext.BaseDirectory, configurado);
                json = File.ReadAllText(credenciaisPath, new UTF8Encoding(false));
            }

            var bytes = Encoding.UTF8.GetBytes(json.TrimStart('\uFEFF'));
            using var stream = new MemoryStream(bytes);
            var credential = GoogleCredential
                .FromStream(stream)
                .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);

            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "InstitutoTriboDeDavi"
            });
        }
    }
}
