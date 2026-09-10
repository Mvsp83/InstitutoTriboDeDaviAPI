# Governança de Dados (LGPD) — complemento técnico

Instituto Tribo de Davi — Lei 13.709/2018. Este documento cobre a camada de
**governança e segurança da infraestrutura**: com quem os dados são
compartilhados, onde ficam, e como estão protegidos em repouso e em trânsito.
Complementa o [RETENCAO_LGPD.md](RETENCAO_LGPD.md) (prazos e direitos do titular)
e o [SEGURANCA_AUTENTICACAO.md](SEGURANCA_AUTENTICACAO.md) (login, 2FA, tokens).

> Não substitui orientação jurídica. Os itens marcados **[organizacional]**
> dependem de uma decisão/documento do Instituto, não do código.

## 1. Inventário de operadores e sub-operadores (art. 39)

Todo terceiro que processa dado pessoal em nome do Instituto é um **operador** e
precisa de contrato de tratamento. O sistema integra com:

| Terceiro | O que trafega | Onde fica (residência) | Base p/ transferência |
|---|---|---|---|
| **Render** (hospedagem da API) | Requisições/processamento | EUA | art. 33 |
| **Neon** (PostgreSQL) | Todo o banco (cadastro, frequência, inscrições, financeiro) | EUA | art. 33 |
| **Google Sheets** | Respostas dos formulários de inscrição (PII de menores) | Google — EUA | art. 33 |
| **Google Drive** | Documentos contábeis (prestação de contas) | Google — EUA | art. 33 |
| **SMTP / Gmail** | Emails de aviso (endereços, conteúdo) | Google — EUA | art. 33 |
| **Web Push (VAPID)** | Endpoint de push do navegador (sem PII direta) | Provedor do navegador | baixo risco |
| **Sentry** (opcional) | Erros da aplicação — `SendDefaultPii: false` | Sentry — EUA | só liga com DSN |

**Ações [organizacional]:**
- [ ] Como Render e Neon ficam nos **EUA**, apoiar a transferência nas cláusulas-padrão/adequação (art. 33).
- [ ] Contrato de operador (DPA) com cada terceiro acima que fique ativo.
- [ ] Para transferências ao exterior (Google/Sentry), apoiar-se nas cláusulas-padrão/adequação (art. 33).
- [ ] Manter `Sentry:SendDefaultPii` em `false` (já é o padrão) para não vazar PII nos relatórios de erro.

## 2. Cifragem em repouso (art. 46)

CPF, RG e endereço ficam em **texto puro** nas colunas do banco. O padrão de
cuidado para dado de menor recomenda cifrar o armazenamento.

**Hoje:** o **Neon cifra o armazenamento em repouso por padrão** (o banco
gerenciado já entrega cifra de disco/backup). Isso cobre o risco de roubo do
arquivo/backup no provedor.

> Cifra por coluna (a nível de aplicação) foi considerada e **descartada por
> ora**: quebraria busca e unicidade por CPF e exigiria gestão de chave própria;
> o ganho sobre a cifra do provedor + controle de acesso não justifica o risco.
> A cifra do provedor não protege contra acesso autenticado ao banco — para isso
> valem o controle por papel e o log de auditoria, que já existem.

- [x] Cifra em repouso no provedor (Neon).

## 3. Cifragem em trânsito

- **Cliente ↔ API:** já coberto — HSTS + redirect HTTPS em produção ([Startup.cs](InstitutoTriboDeDavi.API/Startup.cs)).
- **API ↔ Neon:** usar TLS na connection string (`SSL Mode=Require`) — o Npgsql
  liga a cifra e valida o servidor gerenciado do Neon.

## 4. Minimização já implementada (não mexer)

- Log de auditoria **nunca grava valores de PII** — só o fato da alteração ([AuditoriaInterceptor](InstitutoTriboDeDavi.Infrastructure/Auditoria)).
- Senhas e refresh tokens só existem em **hash**.
- Cache offline no navegador é **limpo no logout** (mitiga PII em dispositivo compartilhado).
- Anonimização é **manual, com revisão humana** — correto para uma operação irreversível; não automatizar em timer.

## 5. Checklist organizacional [organizacional]

Estes itens decidem a conformidade e vivem fora do código:

- [ ] **Encarregado (DPO)** designado e contato publicado no site (art. 41).
- [ ] **Política de Privacidade** entregue às famílias em linguagem acessível (art. 9).
- [ ] **RIPD** (Relatório de Impacto) — recomendável por tratar dado sensível de criança (art. 38).
- [ ] **Plano de resposta a incidente**: como notificar ANPD e titulares em caso de vazamento (art. 48).
- [ ] **Registro das operações de tratamento** (art. 37) — este documento + RETENCAO_LGPD.md são a base.
- [ ] Revisar este inventário a cada nova integração.
