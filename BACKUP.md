# Backup e restauração do banco

O banco guarda cadastro de alunos, presenças, planos, patrimônio e o
**financeiro do instituto** — dados de prestação de contas que não existem em
outro lugar. Sem backup testado, um disco com defeito apaga a história da ONG.

> **Regra que vale mais que o script:** um backup só existe depois de ter sido
> restaurado com sucesso pelo menos uma vez. Faça o teste da seção
> [Restauração](#restauração) hoje, não no dia do problema.

## A regra 3-2-1 (versão realista para a ONG)

| | O que fazer | Custo |
|---|---|---|
| **3 cópias** | O banco em produção + backup local + backup na nuvem | R$ 0 |
| **2 mídias** | Disco do servidor + pendrive/HD externo *ou* nuvem | R$ 0 |
| **1 fora do local** | Google Drive / OneDrive da instituição | R$ 0 (contas gratuitas) |

O ponto crítico é a **cópia fora do servidor**: incêndio, roubo ou
ransomware levam junto tudo o que estiver só na mesma máquina.

## Backup automático (SQL Server, Windows)

O script [`scripts/backup-banco.ps1`](scripts/backup-banco.ps1) gera o `.bak`,
**verifica a integridade** (`RESTORE VERIFYONLY`), compacta em `.zip` e apaga
backups com mais de 30 dias — o expurgo só roda depois que o backup novo passa
na verificação.

### Teste manual

```powershell
.\scripts\backup-banco.ps1 -Servidor "localhost" -Banco "TriboDeDavi" -Destino "D:\Backups\Tribo"
```

### Agendar para rodar todo dia

No **Agendador de Tarefas do Windows**:

1. *Criar Tarefa* → nome "Backup Tribo de Davi";
2. Marque **Executar estando o usuário conectado ou não**;
3. *Disparadores* → Diariamente, ex.: **02:00**;
4. *Ações* → Iniciar um programa:
   - Programa: `powershell.exe`
   - Argumentos:
     ```
     -ExecutionPolicy Bypass -File "C:\caminho\InstitutoTriboDeDaviAPI\scripts\backup-banco.ps1" -Servidor "localhost" -Banco "TriboDeDavi" -Destino "D:\Backups\Tribo"
     ```
5. Depois de criar, **execute uma vez manualmente** e confira `backup.log` no
   destino.

### Levar para fora do servidor

Aponte o `-Destino` para uma pasta sincronizada (Google Drive/OneDrive), ou
sincronize depois:

```powershell
robocopy "D:\Backups\Tribo" "G:\Meu Drive\Backups\Tribo" *.zip /MIR /R:2 /W:5
```

## Restauração

Restaure num banco **de teste** (nunca por cima do de produção) e confira se os
dados aparecem no portal.

```sql
-- 1) Descubra os nomes lógicos dos arquivos dentro do backup
RESTORE FILELISTONLY FROM DISK = N'D:\Backups\Tribo\TriboDeDavi_2026-08-21_0200.bak';

-- 2) Restaure com outro nome, apontando para caminhos novos
RESTORE DATABASE [TriboDeDavi_TESTE]
FROM DISK = N'D:\Backups\Tribo\TriboDeDavi_2026-08-21_0200.bak'
WITH MOVE 'TriboDeDavi'     TO 'D:\SQLData\TriboDeDavi_TESTE.mdf',
     MOVE 'TriboDeDavi_log' TO 'D:\SQLData\TriboDeDavi_TESTE_log.ldf',
     RECOVERY, STATS = 10;
```

Depois confira, por exemplo:

```sql
SELECT COUNT(*) FROM ALUNO;
SELECT COUNT(*) FROM MOVIMENTACAO_FINANCEIRA;
SELECT TOP 5 * FROM MOVIMENTACAO_FINANCEIRA ORDER BY Data DESC;
```

Faça esse teste **a cada 3 meses** e anote a data na tabela abaixo.

| Data do teste | Quem fez | Resultado |
|---|---|---|
| | | |

## Se migrar para PostgreSQL

O roadmap sugere trocar SQL Server por PostgreSQL para cortar custo de
hospedagem. Nesse caso o backup fica ainda mais simples, e os serviços
gerenciados (Supabase, Neon, Railway) já fazem backup automático no plano
gratuito — confira a retenção oferecida, que costuma ser de poucos dias.

```bash
# dump diário compactado
pg_dump "$DATABASE_URL" --format=custom --file="tribo_$(date +%F).dump"

# restauração num banco de teste
pg_restore --dbname="$URL_TESTE" --clean --if-exists tribo_2026-08-21.dump
```

## O que **não** é backup

- **Réplica / espelhamento**: apaga um dado por engano e o erro se replica na hora.
- **`localStorage` do navegador**: era onde o financeiro vivia antes; limpar o
  cache apagava tudo (por isso ele foi migrado para o banco).
- **Backup nunca restaurado**: enquanto não for testado, é só um arquivo grande.
