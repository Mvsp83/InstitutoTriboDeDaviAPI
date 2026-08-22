# Autenticação: 2FA e refresh tokens (A4)

Duas camadas somadas ao login por senha: **2FA (TOTP)** opcional por usuário e
**refresh tokens revogáveis** no servidor. O desenho respeita o uso **offline**
no tatame (a sessão sobrevive sem rede).

## Fluxo de login

`POST /api/v1/auth/login` `{ login, password, codigo2fa? }`:

1. Valida login + senha. Errado → 401.
2. Se o usuário **tem 2FA** e `codigo2fa` veio vazio → responde
   `{ requer2fa: true }` (sem token). O front pede o código e repete a chamada.
3. Com o código válido (ou sem 2FA) → emite **access token (JWT, 8h)** +
   **refresh token** e devolve `{ token, tokenExpires, refreshToken }`.

## Refresh tokens (renovação + revogação)

- Guardados no servidor (tabela `REFRESH_TOKENS`) apenas como **hash SHA-256** —
  um vazamento do banco não devolve tokens usáveis. Validade: **30 dias**.
- `POST /api/v1/auth/refresh` `{ refreshToken }` → valida, **rotaciona** (revoga
  o usado, emite um novo) e devolve um novo access token. Reusar um refresh já
  rotacionado é recusado (defesa contra roubo).
- `POST /api/v1/auth/logout` `{ refreshToken }` → revoga o token (logout real).
- `POST /api/Usuario/{id}/revogar-sessoes` (**Administrador**) → revoga **todas**
  as sessões de um usuário: "sair de todos os aparelhos" e ao desativar alguém.
- Usuário excluído: o refresh falha no `Get` seguinte → renovação negada.

### Por que não cookie httpOnly + access token em memória

O padrão "livro-texto" quebraria a **chamada offline**: o professor reabre o PWA
no tatame sem rede e precisa autenticar. Por isso o JWT (e o refresh) seguem em
`localStorage` — a sessão sobrevive ao fechamento e funciona offline; ao voltar
a conexão, o refresh renova silenciosamente (interceptor no `api.ts`). O ganho de
segurança aqui é a **revogação** no servidor, não a blindagem contra XSS.

## 2FA (TOTP) — autogerido pelo usuário

Padrão RFC 6238 (Google Authenticator/Authy). Sem dependência de SMTP/SMS.
Endpoints sobre o usuário autenticado (`[Authorize]`), acessíveis no portal em
**menu do avatar → Verificação em 2 etapas**:

- `GET  /api/Usuario/2fa/status` → `{ ativo }`.
- `POST /api/Usuario/2fa/iniciar` → gera secret + URI otpauth (vira QR). O 2FA
  **só passa a valer após confirmar**.
- `POST /api/Usuario/2fa/confirmar` `{ codigo }` → valida o primeiro código e ativa.
- `POST /api/Usuario/2fa/desativar` `{ codigo }` → exige um código válido para
  desligar (uma sessão sequestrada não desativa a proteção sozinha).

O secret fica em `USUARIOS.TotpSecret` (nulo = sem 2FA); `TotpConfirmado` indica
que o login passa a exigir o segundo fator.

## Verificação end-to-end (quando a API estiver no ar)

1. Login normal → recebe token + refreshToken.
2. Ativar 2FA no menu do avatar, escanear o QR, confirmar → sair → no novo login,
   após a senha, o portal pede o código de 6 dígitos.
3. Deixar o access token expirar (ou forjar 401) com a rede ligada → uma
   requisição dispara `/refresh` e segue sem novo login.
4. `revogar-sessoes` de um usuário → o refresh dele para de renovar.
