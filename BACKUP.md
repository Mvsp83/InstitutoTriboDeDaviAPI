# Backup e restauração do banco

O banco guarda cadastro de alunos, presenças, planos, patrimônio e o
**financeiro do instituto** — dados de prestação de contas que não existem em
outro lugar. Sem backup testado, uma falha apaga a história da ONG.

> **Regra que vale mais que qualquer script:** um backup só existe depois de ter
> sido restaurado com sucesso pelo menos uma vez. Faça o teste da seção
> [Restauração](#restauração) hoje, não no dia do problema.

Produção: **PostgreSQL no Neon**.

## 1. Backup gerenciado (Neon)

O Neon faz backup contínuo e permite **restauração a um ponto no tempo**
(*point-in-time restore*) e **branches** do banco — confira a **retenção** do
seu plano (no gratuito costuma ser de poucos dias). É a primeira linha de
defesa, mas **não substitui uma cópia sua fora do provedor**: uma conta suspensa
ou apagada leva junto os backups do provedor.

## 2. Cópia própria, fora do provedor (a que importa)

Um `pg_dump` periódico, guardado em outro lugar (Google Drive/OneDrive da
instituição):

```bash
# dump compactado (formato custom)
pg_dump "$DATABASE_URL" --format=custom --file="tribo_$(date +%F).dump"
```

- `DATABASE_URL` = a connection string do Neon (a mesma da API).
- Guarde o arquivo **fora do provedor** — incêndio, roubo, ransomware ou uma
  conta perdida levam junto tudo o que estiver só num lugar.
- Mantenha os últimos ~30 dias e apague os mais antigos.

## Restauração

Restaure num banco **de teste** (nunca por cima do de produção) e confira se os
dados aparecem no portal.

```bash
pg_restore --dbname="$URL_TESTE" --clean --if-exists tribo_2026-08-21.dump
```

Depois confira, por exemplo:

```sql
SELECT COUNT(*) FROM "ALUNO";
SELECT COUNT(*) FROM "MOVIMENTACAO_FINANCEIRA";
```

Faça esse teste **a cada 3 meses** e anote a data abaixo.

| Data do teste | Quem fez | Resultado |
|---|---|---|
| | | |

## O que **não** é backup

- **Réplica / espelhamento**: apaga um dado por engano e o erro se replica na hora.
- **`localStorage` do navegador**: era onde o financeiro vivia antes; limpar o
  cache apagava tudo (por isso foi migrado para o banco).
- **Backup nunca restaurado**: enquanto não for testado, é só um arquivo grande.
