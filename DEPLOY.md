# Guia de Publicação — API Instituto Tribo de Davi

Checklist para colocar a API em produção. O código já está preparado
(hardening da "Fase 1"); este guia cobre o que é feito **no servidor**.

## 1. Variáveis de ambiente obrigatórias

Definir no painel do provedor (App Service, Render, VPS etc.):

| Variável | Valor |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__TRIBODEDAVIAPI` | Connection string do banco de produção (usuário SQL dedicado, **não** usar `sa`) |
| `Jwt__Key` | Chave **nova**, gerada no passo 2 |
| `Jwt__Issuer` | URL pública da API, ex.: `https://api.tribodedavi.org.br` |
| `Jwt__Audience` | Identificador do cliente, ex.: `tribodedavi-app` |

Comportamentos que dependem do ambiente `Production`:
- Swagger desligado;
- `RequireHttpsMetadata` ligado e redirecionamento para HTTPS ativo (com suporte a proxy/`X-Forwarded-*`);
- Validação de Issuer/Audience do JWT ativa (porque as variáveis acima existem);
- CORS fechado (nenhuma origem liberada — o app mobile não usa CORS).

## 2. Gerar a chave JWT de produção

Nunca reutilizar a chave de desenvolvimento. Gerar uma nova (PowerShell):

```powershell
[Convert]::ToBase64String((1..64 | ForEach-Object { Get-Random -Maximum 256 }))
```

Guardar apenas na variável de ambiente `Jwt__Key`. Trocar a chave invalida
todos os tokens emitidos (usuários precisam logar de novo).

## 3. Credenciais do Google Sheets

O arquivo `credentials/tribo-de-davi-sheets.json` **não está no repositório**
(de propósito). Enviar manualmente para o servidor, na pasta indicada por
`GoogleSheets:CredenciaisJson` (relativa ao diretório da aplicação publicada).

## 4. Banco de dados

1. Criar o banco no destino;
2. Restaurar um backup do `TRIBODEDAVIAPI` local (traz usuários, aulas e
   presenças — dados que não vêm do Sheets);
3. Criar usuário SQL dedicado com permissão apenas nesse banco;
4. Configurar backup automático + testar um restore.

As migrations do EF estão no projeto `InstitutoTriboDeDavi.Infrastructure`;
para criar um banco vazio do zero: `dotnet ef database update`.

## 5. Publicar

```powershell
dotnet publish InstitutoTriboDeDavi.API -c Release -o ./publish
```

(ou deploy via Visual Studio / GitHub Actions, conforme o provedor)

## 6. Smoke test pós-deploy

1. `GET /health` → `Healthy` (verifica app + conexão com o banco);
2. `POST /api/v1/auth/login` com um usuário real → token;
3. `GET /api/aluno/get-por-polo?turmas=1` com o token → dados;
4. Verificar no dia seguinte o histórico da sincronização noturna
   (`GET /api/sincronizacao/historico/ultima-execucao`) — ela roda às 02:00
   **horário de Brasília**, independente do fuso do servidor.

## 7. App Flutter (APK para os professores)

```powershell
flutter build apk --release --dart-define=API_BASE_URL=https://SUA-URL/api
```

- Criar o keystore de assinatura na primeira vez e **guardá-lo com backup**
  (perder o keystore impede publicar atualizações do mesmo app);
- Testar o APK em 1–2 aparelhos reais antes de distribuir ao grupo.
