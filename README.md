# Instituto Tribo de Davi — API

API REST do Instituto Tribo de Davi (projeto social de jiu-jitsu). Atende o
**portal administrativo** e o **site público** ([TriboDeDaviWeb](https://github.com/Mvsp83/TriboDeDaviWeb),
React), cobrindo alunos, aulas/presenças, graduação, inscrições públicas,
financeiro, loja, doações, transparência e o portal do responsável.

## Stack

- **.NET 8** (ASP.NET Core Web API) em **Clean Architecture**
- **Entity Framework Core** + **PostgreSQL** (produção: Neon)
- **JWT** (login por senha, **2FA/TOTP** opcional e **refresh tokens** revogáveis)
- **Serilog** (logs) + **Sentry** (erros, opt-in) · **Swagger** (só fora de produção)
- Integrações opcionais: **Google Sheets/Drive**, **SMTP** (avisos), **Web Push**

## Projetos

| Projeto | Responsabilidade |
|---|---|
| `InstitutoTriboDeDavi.Domain` | Entidades e regras de domínio |
| `InstitutoTriboDeDavi.Application` | Serviços, DTOs e interfaces (casos de uso) |
| `InstitutoTriboDeDavi.Infrastructure` | EF Core (contexto, mapeamentos, migrations), repositórios e integrações |
| `InstitutoTriboDeDavi.API` | Controllers, autenticação, jobs de background |
| `InstitutoTriboDeDavi.Tests` | Testes (xUnit) |

## Rodando em desenvolvimento

Requer **.NET 8 SDK** e um **PostgreSQL** acessível.

```bash
dotnet restore
# defina a connection string (appsettings.Development.json ou variável de ambiente):
#   ConnectionStrings__TRIBODEDAVIAPI = "Host=...;Database=...;Username=...;Password=..."
dotnet run --project InstitutoTriboDeDavi.API
```

As **migrations do EF são aplicadas automaticamente na inicialização**
(`Database.Migrate()`), então um banco vazio é criado/atualizado ao subir a app.
Swagger fica em `/swagger` fora de `Production`.

## Testes

```bash
dotnet test
```

## Documentação por tópico

- [DEPLOY.md](DEPLOY.md) — publicação em produção (Render + Neon)
- [BACKUP.md](BACKUP.md) — backup e restauração do banco
- [SEGURANCA_AUTENTICACAO.md](SEGURANCA_AUTENTICACAO.md) — 2FA e refresh tokens
- [OBSERVABILIDADE.md](OBSERVABILIDADE.md) — Serilog e Sentry
- [GOVERNANCA_LGPD.md](GOVERNANCA_LGPD.md) · [RETENCAO_LGPD.md](RETENCAO_LGPD.md) — LGPD
- [PORTAL_RESPONSAVEL.md](PORTAL_RESPONSAVEL.md) — área do responsável (acesso por código)
- [EMAIL_SETUP.md](EMAIL_SETUP.md) — SMTP dos avisos do calendário
- [GOOGLE_DRIVE_SETUP.md](GOOGLE_DRIVE_SETUP.md) — armazenamento dos documentos
- [DESLIGAMENTO_FLUTTER.md](DESLIGAMENTO_FLUTTER.md) — decomissionamento do app Flutter (legado)
