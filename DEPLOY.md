# Publicação — API Instituto Tribo de Davi

Produção atual: **Render** (serviço Docker `tribodedavi-api`) + **Neon**
(PostgreSQL). O portal/site é um serviço estático separado (`tribodedavi-web`).

## 1. Variáveis de ambiente

Definir no painel do provedor (Render → Environment). Chaves aninhadas usam `__`.

| Variável | Valor |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__TRIBODEDAVIAPI` | Connection string do Postgres (Neon), ex.: `Host=...;Database=...;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true` |
| `Jwt__Key` | Chave **nova** (ver passo 2) |
| `Jwt__Issuer` | URL pública da API |
| `Jwt__Audience` | Identificador do cliente, ex.: `tribodedavi-app` |
| `Cors__AllowedOrigins__0` | URL pública do portal/site (ex.: `https://tribodedavi-web.onrender.com`). Sem isso, o navegador bloqueia as chamadas do site. |

Comportamentos ligados por `ASPNETCORE_ENVIRONMENT=Production`:
- Swagger desligado;
- HTTPS obrigatório (com suporte a proxy/`X-Forwarded-*`);
- Validação de Issuer/Audience do JWT.

Integrações opcionais (só se a funcionalidade for usada): `Smtp__*`
(avisos por email — ver [EMAIL_SETUP.md](EMAIL_SETUP.md)), `GoogleDrive__*`
(documentos — ver [GOOGLE_DRIVE_SETUP.md](GOOGLE_DRIVE_SETUP.md)), `WebPush__*`
(notificações), `Sentry__Dsn` (erros — ver [OBSERVABILIDADE.md](OBSERVABILIDADE.md)).

## 2. Gerar a chave JWT de produção

Nunca reutilizar a chave de desenvolvimento. Gerar uma nova (PowerShell):

```powershell
[Convert]::ToBase64String((1..64 | ForEach-Object { Get-Random -Maximum 256 }))
```

Guardar apenas na variável `Jwt__Key`. Trocar a chave invalida todos os tokens
emitidos (usuários precisam logar de novo).

## 3. Banco de dados

As **migrations do EF são aplicadas automaticamente ao subir a API**
(`Database.Migrate()` no startup) — não é preciso rodar nada à mão para um banco
novo. Recomendado configurar **backup automático** e **testar um restore**
(ver [BACKUP.md](BACKUP.md)).

## 4. Deploy

O Render faz **auto-deploy** a cada push no `master` (build via Dockerfile:
`dotnet publish -c Release`). Para publicar manualmente em outro provedor:

```powershell
dotnet publish InstitutoTriboDeDavi.API -c Release -o ./publish
```

## 5. Smoke test pós-deploy

1. `GET /health` → `Healthy` (verifica app + conexão com o banco);
2. `POST /api/v1/auth/login` com um usuário real → token;
3. Abrir o site público e uma tela autenticada do portal (confirma o CORS).

---

> **Legado:** o app **Flutter** (chamada) está descontinuado — ver
> [DESLIGAMENTO_FLUTTER.md](DESLIGAMENTO_FLUTTER.md). A **sincronização** noturna
> com Google Sheets também está em processo de aposentadoria (a importação
> inicial de alunos passa a ser feita pela própria tela de importação).
