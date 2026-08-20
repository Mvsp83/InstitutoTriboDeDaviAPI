# Configuração do Google Drive (documentos)

As telas de **Documentos** do portal — tanto os **Contábeis** (DRE, Balanço,
Relatório de Atividades) quanto os **Modelos de Documentos** — armazenam os
arquivos no **Google Drive** de uma conta Gmail comum, via OAuth 2.0.

Enquanto as credenciais não forem configuradas, o upload/listagem falha com:

> Integração com o Google Drive não configurada
> (GoogleDrive:ClientId/ClientSecret/RefreshToken ausentes).

Configurar isto **uma vez** destrava **todas** as telas de documentos de uma vez.

---

## Como funciona

- O servidor age **como o próprio usuário** (conta Gmail), não como conta de
  serviço — contas de serviço não têm cota de armazenamento no Drive pessoal.
- Escopo usado: **`drive.file`** — o app só enxerga/gerencia os arquivos que
  ele mesmo cria. Ele **não** tem acesso ao resto do seu Drive.
- A pasta raiz (`Instituto Tribo de Davi - Documentos`) e as subpastas por
  categoria (`DRE`, `Balanço`, `Relatório de Atividades`, `Modelos de
  Documentos`) são **criadas automaticamente** no primeiro upload.

As 3 chaves necessárias (`GoogleDriveConfig`):

| Chave          | O que é                                            |
| -------------- | -------------------------------------------------- |
| `ClientId`     | ID do cliente OAuth (Google Cloud)                 |
| `ClientSecret` | Segredo do cliente OAuth                           |
| `RefreshToken` | Token de atualização obtido no consentimento único |

`PastaRaiz` já tem padrão no `appsettings.json` e normalmente não precisa mudar.

---

## Passo 1 — Criar as credenciais OAuth (Google Cloud Console)

1. Acesse <https://console.cloud.google.com> e crie/selecione um projeto.
2. **APIs e serviços → Biblioteca →** ative a **Google Drive API**.
3. **APIs e serviços → Tela de consentimento OAuth**:
   - Tipo de usuário: **Externo**.
   - Preencha nome do app, e-mail de suporte e de contato.
   - **Publique a tela como "Em produção"** (botão *Publicar app*).
     > Importante: enquanto a tela fica em modo *Teste*, o refresh token
     > **expira em ~7 dias** e os uploads voltam a falhar. Em *Produção* ele
     > não expira (até ser revogado).
4. **APIs e serviços → Credenciais → Criar credenciais → ID do cliente OAuth**:
   - Tipo de aplicativo: **App para computador** (Desktop) **ou** **Aplicativo
     da Web**.
   - Se escolher **Aplicativo da Web**, adicione em *URIs de redirecionamento
     autorizados*:
     `https://developers.google.com/oauthplayground`
     (necessário para o Passo 2 via Playground).
   - Anote o **ClientId** e o **ClientSecret**.

---

## Passo 2 — Obter o RefreshToken (OAuth Playground)

1. Acesse <https://developers.google.com/oauthplayground>.
2. Clique na **engrenagem** (canto superior direito):
   - Marque **Use your own OAuth credentials**.
   - Cole o **ClientId** e o **ClientSecret** do Passo 1.
3. No painel esquerdo, em *Step 1*, cole o escopo abaixo e clique
   **Authorize APIs**:
   ```
   https://www.googleapis.com/auth/drive.file
   ```
4. Faça login com a **conta Gmail** que vai guardar os documentos e conceda o
   acesso.
5. Em *Step 2*, clique **Exchange authorization code for tokens**.
6. Copie o valor de **`refresh_token`** que aparece.

> Faça isso logado na conta correta — os arquivos ficarão no Drive **dessa**
> conta.

---

## Passo 3 — Gravar os segredos

Os segredos **não** vão para o `appsettings.json` (ele fica versionado).

### Desenvolvimento — user-secrets

Rode no seu terminal, na raiz do repositório, trocando os `...` pelos valores
reais:

```bash
dotnet user-secrets set "GoogleDrive:ClientId" "..." --project InstitutoTriboDeDavi.API
dotnet user-secrets set "GoogleDrive:ClientSecret" "..." --project InstitutoTriboDeDavi.API
dotnet user-secrets set "GoogleDrive:RefreshToken" "..." --project InstitutoTriboDeDavi.API
```

Conferir o que está gravado:

```bash
dotnet user-secrets list --project InstitutoTriboDeDavi.API
```

> Observação: rode estes comandos **no seu próprio terminal** (ou defina via
> Visual Studio → *Gerenciar segredos do usuário*). Ferramentas executando em
> ambiente virtualizado (MSIX) podem gravar o `secrets.json` num cache isolado
> que o `dotnet`/Visual Studio "de verdade" não enxergam.

### Produção — variáveis de ambiente

Use `__` (dois underscores) no lugar de `:`:

```
GoogleDrive__ClientId=...
GoogleDrive__ClientSecret=...
GoogleDrive__RefreshToken=...
```

---

## Passo 4 — Reiniciar e testar

1. Reinicie a API:
   ```bash
   dotnet run --project InstitutoTriboDeDavi.API
   ```
2. No portal, como **Administrador**, abra **Modelos de Documentos** (ou uma
   tela contábil) e envie um PDF de teste.
3. Confira no Drive da conta: deve surgir a pasta
   `Instituto Tribo de Davi - Documentos` com a subpasta da categoria e o
   arquivo dentro.

---

## Solução de problemas

| Sintoma                                             | Causa provável                                                                 |
| --------------------------------------------------- | ------------------------------------------------------------------------------ |
| "Integração com o Google Drive não configurada"     | Alguma das 3 chaves está vazia / não foi lida (ver *user-secrets list*).        |
| Uploads param de funcionar depois de alguns dias    | Tela de consentimento ainda em **Teste** — publique "Em produção".              |
| `invalid_grant` ao usar                             | RefreshToken revogado ou de outra conta — gere de novo (Passo 2).               |
| `insufficient scopes` / 403                         | Escopo diferente de `drive.file` na hora de gerar o token — refaça com o certo. |
| Segredos setados mas API não enxerga                | `secrets.json` gravado em cache virtualizado — rode os comandos no seu terminal.|

---

## Categorias de documentos

Definidas no enum `CategoriaDocumento`
(`InstitutoTriboDeDavi.Domain/Enums/CategoriaDocumento.cs`). O `Description` de
cada uma vira o nome da subpasta no Drive:

| Enum                  | Subpasta no Drive          |
| --------------------- | -------------------------- |
| `Dre`                 | DRE                        |
| `Balanco`             | Balanço                    |
| `RelatorioAtividades` | Relatório de Atividades    |
| `Modelos`             | Modelos de Documentos      |

Para adicionar uma nova categoria, basta acrescentar um valor ao enum com seu
`[Description(...)]` — a subpasta é criada sozinha.
