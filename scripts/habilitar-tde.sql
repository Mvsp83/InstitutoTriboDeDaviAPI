/*
====================================================================
 Cifragem em repouso do banco (TDE — Transparent Data Encryption)
====================================================================
 Instituto Tribo de Davi — proteção de dados pessoais de menores (LGPD, art. 46).

 O QUE FAZ
   Liga o TDE no banco TRIBODEDAVIAPI. A partir daí os arquivos de dados (.mdf),
   de log (.ldf) e os backups ficam cifrados em disco: quem copiar o arquivo do
   banco não consegue lê-lo sem o certificado. É transparente para a aplicação —
   nenhuma mudança de código, query ou connection string.

 O QUE **NÃO** FAZ
   TDE cifra o arquivo, não a coluna. Um usuário com acesso ao banco continua
   lendo o CPF normalmente — para isso já valem o controle de acesso por papel e
   o log de auditoria. TDE protege contra roubo do arquivo/backup, não contra
   acesso autenticado.

 PRÉ-REQUISITOS
   - SQL Server Enterprise, ou Standard 2019+ (o TDE saiu do "só Enterprise" no
     SQL Server 2019). Azure SQL Database já vem com TDE ligado por padrão — lá
     este script é dispensável.
   - Rodar como sysadmin, conectado ao servidor de produção.

 ⚠️  BACKUP DO CERTIFICADO — LEIA ANTES DE RODAR
   Se você perder o certificado (passo 2) e sua senha, os backups cifrados ficam
   IRRECUPERÁVEIS. O passo 3 exporta o certificado para arquivo: guarde esse
   arquivo E a senha num cofre separado do servidor. Sem isso, um restore em
   outra máquina é impossível.
====================================================================
*/

-- Ajuste se o nome do banco for diferente neste ambiente.
DECLARE @Banco SYSNAME = N'TRIBODEDAVIAPI';

-- ── 1. Chave-mestra do servidor (uma vez por instância) ────────────────────
--    Troque a senha por uma forte e guarde-a no cofre.
IF NOT EXISTS (SELECT 1 FROM master.sys.symmetric_keys WHERE name = '##MS_DatabaseMasterKey##')
BEGIN
    USE master;
    CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'TROQUE_por_uma_senha_forte_e_guarde_no_cofre';
    PRINT 'Chave-mestra criada.';
END
ELSE
    PRINT 'Chave-mestra já existe — mantida.';
GO

-- ── 2. Certificado que protege a chave do banco ────────────────────────────
USE master;
IF NOT EXISTS (SELECT 1 FROM sys.certificates WHERE name = 'TDE_Cert_TriboDeDavi')
BEGIN
    CREATE CERTIFICATE TDE_Cert_TriboDeDavi
        WITH SUBJECT = 'Certificado TDE - Instituto Tribo de Davi';
    PRINT 'Certificado criado.';
END
ELSE
    PRINT 'Certificado já existe — mantido.';
GO

-- ── 3. BACKUP DO CERTIFICADO (NÃO PULE) ────────────────────────────────────
--    Ajuste os caminhos. Copie os dois arquivos para um cofre FORA do servidor.
USE master;
BACKUP CERTIFICATE TDE_Cert_TriboDeDavi
    TO FILE = 'C:\backup-tde\TDE_Cert_TriboDeDavi.cer'
    WITH PRIVATE KEY (
        FILE = 'C:\backup-tde\TDE_Cert_TriboDeDavi.pvk',
        ENCRYPTION BY PASSWORD = 'TROQUE_outra_senha_forte_para_o_arquivo'
    );
PRINT 'Certificado exportado — guarde os arquivos .cer e .pvk no cofre.';
GO

-- ── 4. Chave de cifragem do banco (DEK) ────────────────────────────────────
DECLARE @sql NVARCHAR(MAX);
SET @sql = N'USE [TRIBODEDAVIAPI];
IF NOT EXISTS (SELECT 1 FROM sys.dm_database_encryption_keys k
              JOIN sys.databases d ON d.database_id = k.database_id
              WHERE d.name = ''TRIBODEDAVIAPI'')
    CREATE DATABASE ENCRYPTION KEY
        WITH ALGORITHM = AES_256
        ENCRYPTION BY SERVER CERTIFICATE TDE_Cert_TriboDeDavi;';
EXEC sp_executesql @sql;
GO

-- ── 5. Liga o TDE ──────────────────────────────────────────────────────────
ALTER DATABASE [TRIBODEDAVIAPI] SET ENCRYPTION ON;
GO

-- ── 6. Verificação ─────────────────────────────────────────────────────────
--    encryption_state = 3 significa "cifrado". 2 = em progresso (aguarde).
SELECT d.name AS Banco,
       k.encryption_state,
       CASE k.encryption_state
            WHEN 0 THEN 'Sem chave'
            WHEN 1 THEN 'Não cifrado'
            WHEN 2 THEN 'Cifrando...'
            WHEN 3 THEN 'Cifrado'
            WHEN 4 THEN 'Trocando chave'
            WHEN 5 THEN 'Descifrando'
       END AS Estado,
       k.percent_complete AS PercentualConcluido
FROM sys.dm_database_encryption_keys k
JOIN sys.databases d ON d.database_id = k.database_id
WHERE d.name = 'TRIBODEDAVIAPI';
GO
