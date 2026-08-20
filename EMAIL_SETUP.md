# Configuração de email (avisos do calendário)

Os avisos agendados do calendário são enviados por **email (SMTP)**. Um job
diário roda no horário configurado, procura eventos cuja data de disparo
(data do evento menos os dias de antecedência) chegou, e envia para os emails
cadastrados no evento (aceita **vários**, separados por vírgula ou `;`).

Enquanto o SMTP não for configurado, os eventos podem ser criados normalmente,
mas o envio falha (e o job tenta de novo no dia seguinte).

## Chaves (seção `Smtp`)

| Chave | Exemplo (Gmail) | Observação |
| --- | --- | --- |
| `Host` | `smtp.gmail.com` | servidor SMTP |
| `Port` | `587` | normalmente 587 (STARTTLS) |
| `EnableSsl` | `true` | |
| `User` | `avisos@suaong.org` | conta que envia — **segredo** |
| `Password` | (senha de app) | **segredo** — no Gmail, use uma *senha de app*, não a senha normal |
| `From` | `avisos@suaong.org` | remetente exibido; vazio = usa `User` |
| `HorarioExecucao` | `07:00` | horário de Brasília em que o job roda |

`Host`, `Port`, `EnableSsl`, `From` e `HorarioExecucao` já ficam no
`appsettings.json` (não são segredos). **`User` e `Password` NÃO vão no
appsettings.**

## Gmail: gerando uma "senha de app"

1. A conta precisa de **verificação em duas etapas** ativada.
2. Acesse <https://myaccount.google.com/apppasswords>, gere uma senha de app
   (16 caracteres) e use-a como `Smtp:Password`.

> Provedores como SendGrid/Mailgun também têm SMTP e um tier gratuito — mesma
> configuração, trocando Host/User/Password.

## Gravar os segredos

### Desenvolvimento — user-secrets (rode no seu terminal)

```bash
dotnet user-secrets set "Smtp:User" "..." --project InstitutoTriboDeDavi.API
dotnet user-secrets set "Smtp:Password" "..." --project InstitutoTriboDeDavi.API
```

O Host/Port podem ficar no appsettings; se preferir, também dá para setá-los via
user-secrets (`Smtp:Host`, `Smtp:Port`).

### Produção — variáveis de ambiente

```
Smtp__Host=smtp.gmail.com
Smtp__User=...
Smtp__Password=...
```

## Testar

1. Reinicie a API.
2. No portal, crie um evento com data de **hoje**, marque **"Enviar aviso por
   email"**, informe seu email e antecedência **0**.
3. No dia seguinte ao horário configurado o aviso é enviado. Para testar na
   hora, ajuste `Smtp:HorarioExecucao` para alguns minutos à frente e reinicie.

## Como funciona (resumo)

- Cada evento guarda: `Notificar`, `EmailsNotificacao` (vários), `DiasAntecedencia`.
- O job (`NotificacaoCalendarioHostedService`) roda 1x/dia; envia quando
  `hoje >= data - DiasAntecedencia` e marca o evento como notificado (não
  reenvia). Editar o evento re-arma o envio.
