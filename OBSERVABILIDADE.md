# Observabilidade (E3)

Logging estruturado com **Serilog** e monitor de erros de produção com
**Sentry** (opt-in). Custo zero: Serilog é local; Sentry só liga quando há DSN.

## Serilog — logs estruturados

Configurado em `Startup.cs` (`builder.Host.UseSerilog(...)`) e lido da seção
`Serilog` do `appsettings.json`. Substitui os providers de logging padrão — os
`ILogger<T>` já existentes passam a fluir por aqui, sem mudança de código.

**Para onde vai:**
- **Console** — durante `dotnet run` / no output do provedor de hospedagem.
- **Arquivo rotativo** — `logs/tribo-<data>.log`, um por dia, retendo os últimos
  14 dias. A pasta `logs/` é criada sozinha e está fora do git (`.gitignore`
  já ignora `*.log`).

**Request logging:** `app.UseSerilogRequestLogging()` grava uma linha por
requisição (método, rota, status, tempo em ms), depois dos forwarded headers —
então em produção o esquema/IP registrados são os reais (atrás de proxy).

Níveis em `Serilog:MinimumLevel` (padrão `Information`; `Microsoft.AspNetCore` e
`EntityFrameworkCore` em `Warning` para não afogar o log em ruído de framework).

## Sentry — erros de produção (opt-in)

Fica **inativo** enquanto `Sentry:Dsn` estiver vazio — nada é enviado. Para
ligar:

1. Crie um projeto gratuito em https://sentry.io e copie o **DSN**.
2. Configure o segredo (nunca no `appsettings.json`):
   - **Dev:** `dotnet user-secrets set "Sentry:Dsn" "<dsn>"`
   - **Produção:** variável de ambiente `Sentry__Dsn`
3. Suba a API. Exceções não tratadas passam a aparecer no painel do Sentry.

Ajustes opcionais na seção `Sentry` do `appsettings.json`:
- `TracesSampleRate` (0.0–1.0) — amostragem de performance; padrão `0.0` (só erros).
- `SendDefaultPii` — padrão `false`. **Mantenha `false`**: são dados de menores
  (LGPD); não envie IP/identificadores do usuário ao Sentry.

## Verificação

`dotnet run` na API e, em outro terminal, `GET /health`. Deve aparecer:
- no console, a linha de request logging (`HTTP GET /health responded 200 in … ms`);
- o arquivo `logs/tribo-<data>.log` criado, com as mesmas linhas.
