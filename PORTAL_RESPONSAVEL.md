# Portal do Responsável (B3)

Área **pública e só-leitura** para a família acompanhar o aluno: frequência,
graduação, avisos e calendário. Sem contas nem e-mail/SMS.

## Como o responsável acessa

1. O **admin/professor** gera um **código de acesso** por aluno (tela de Alunos
   → botão da chave 🔑) e compartilha com a família (WhatsApp).
2. A família abre **`/responsavel`** no portal e entra com **código + data de
   nascimento do aluno** (segundo fator leve, contra chute de código).
3. A API valida e emite um **token só-leitura de 4h**, escopado àquele aluno.

O código fica em `ALUNOS.CodigoResponsavel` (nulo = acesso não liberado); gerar
um novo invalida o anterior. Alfabeto sem caracteres ambíguos (sem 0/O/1/I/L).

## Endpoints

| Método | Rota | Acesso |
|---|---|---|
| POST | `/api/Aluno/{id}/codigo-responsavel` | Professor+ — (re)gera o código |
| GET  | `/api/Aluno/{id}/codigo-responsavel` | Professor+ — consulta o código |
| POST | `/api/Responsavel/acesso` | público (rate-limit de login) — `{ codigo, dataNascimento }` → token |
| GET  | `/api/Responsavel/painel` | papel **Responsavel** — dados do aluno do token |

## Segurança

- O token do responsável tem papel **`Responsavel`**, que **não casa** com
  Administrador/Supervisor/Professor — logo é inerte contra os endpoints
  internos (o `ObterUsuarioAutenticado` o trata como `Default`, sem privilégio).
- O painel lê o `AlunoId` da claim do token — o responsável só vê o próprio filho.
- No front, o token do responsável fica em `sessionStorage` (separado do admin)
  e usa um cliente HTTP próprio, sem o refresh do portal administrativo.

## Escopo dos avisos

O portal mostra apenas os avisos ativos de público-alvo **"Todos"** (0). Os
avisos direcionados a Professores/Supervisores não aparecem para a família. Se
quiser um canal dedicado, criar um público-alvo "Responsáveis" no cadastro de
avisos é a evolução natural.
