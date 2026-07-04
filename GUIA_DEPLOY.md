# Guia de Deploy — Instituto Tribo de Davi API
## Render (API) + FreeSQLDatabase (SQL Server)

---

## PASSO 1 — Banco de dados no FreeSQLDatabase (5 min)

1. Acesse https://www.freesqldatabase.com
2. Clique em **"Sign Up"** e crie uma conta gratuita
3. Após login, clique em **"Create Database"**
4. Escolha **Microsoft SQL Server**
5. Anote as informações geradas:
   - **Server** (ex: `sql5.freesqldatabase.com`)
   - **Database name** (ex: `sql5123456`)
   - **Username**
   - **Password**

6. Monte a sua connection string no formato:
   ```
   Server=SEU_SERVER;Database=SEU_DATABASE;User Id=SEU_USER;Password=SUA_SENHA;TrustServerCertificate=true;
   ```

7. Acesse o banco via **Azure Data Studio** ou **DBeaver** e rode o script SQL
   para criar as tabelas (o mesmo que você usa localmente).

---

## PASSO 2 — Preparar o projeto para o GitHub (5 min)

1. Coloque os arquivos `Dockerfile` e `.dockerignore` na **raiz da solution**,
   no mesmo nível do arquivo `InstitutoTriboDaviAPI.sln`

2. Coloque o `appsettings.Production.json` dentro da pasta
   `InstitutoTriboDeDavi.API/`

3. Suba o projeto para um repositório **privado** no GitHub:
   - Acesse https://github.com/new
   - Crie um repositório privado chamado `InstitutoTriboDaviAPI`
   - Faça o push do código

   ```bash
   git init
   git add .
   git commit -m "primeiro commit"
   git branch -M main
   git remote add origin https://github.com/SEU_USUARIO/InstitutoTriboDaviAPI.git
   git push -u origin main
   ```

---

## PASSO 3 — Deploy no Render (10 min)

1. Acesse https://render.com e crie uma conta gratuita (pode usar conta GitHub)

2. No painel, clique em **"New +"** → **"Web Service"**

3. Conecte sua conta do GitHub e selecione o repositório `InstitutoTriboDaviAPI`

4. Configure o serviço:
   | Campo | Valor |
   |-------|-------|
   | **Name** | instituto-tribo-de-davi-api |
   | **Region** | Oregon (US West) ou São Paulo se disponível |
   | **Branch** | main |
   | **Runtime** | Docker |
   | **Instance Type** | Free |

5. Clique em **"Advanced"** e adicione as **variáveis de ambiente**:

   | Key | Value |
   |-----|-------|
   | `ASPNETCORE_ENVIRONMENT` | `Production` |
   | `ConnectionStrings__TRIBODEDAVIAPI` | `Server=...` (sua connection string completa) |
   | `Jwt__Key` | `(gerar chave nova — ver DEPLOY.md, passo 2)` |
   | `Jwt__Login` | `teste` |
   | `Jwt__Password` | `teste` |
   | `Jwt__HoursToExpire` | `1` |

   > ⚠️ Note os **dois underscores** (`__`) — é como o .NET lê seções aninhadas
   > de variáveis de ambiente.

6. Clique em **"Create Web Service"**

7. O Render vai fazer o build do Docker automaticamente. Acompanhe o log.
   O primeiro deploy demora ~5 minutos.

---

## PASSO 4 — Testar a API (2 min)

1. Após o deploy, o Render mostrará uma URL como:
   ```
   https://instituto-tribo-de-davi-api.onrender.com
   ```

2. Acesse o Swagger para testar:
   ```
   https://instituto-tribo-de-davi-api.onrender.com/swagger
   ```
   > **Atenção:** o Swagger só aparece se `ASPNETCORE_ENVIRONMENT` for `Development`.
   > Para ativar em produção, veja a seção abaixo.

3. No seu aplicativo mobile, troque a URL base da API para a URL do Render.

---

## Ativar Swagger em Produção (opcional, só para testes)

No arquivo `Startup.cs`, mude:

```csharp
// De:
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Para:
app.UseSwagger();
app.UseSwaggerUI();
```

---

## ⚠️ Limitações do plano gratuito do Render

- A API **"dorme"** após 15 minutos sem receber requests.
- O primeiro request depois de dormir demora ~30 segundos (cold start).
- Para testes isso é suficiente. Para produção real, considere o plano pago ($7/mês).

---

## Dúvidas comuns

**A API não conecta ao banco:**
- Verifique se a connection string na variável de ambiente está correta
- O FreeSQLDatabase libera acesso de qualquer IP por padrão

**Erro de build no Render:**
- Verifique se o `Dockerfile` está na raiz da solution (mesmo nível do `.sln`)
- Verifique os logs de build no painel do Render

**Google Sheets não funciona:**
- Adicione o conteúdo do arquivo `credentials/tribo-de-davi-sheets.json`
  como variável de ambiente, ou inclua o arquivo no repositório (cuidado com segurança)
