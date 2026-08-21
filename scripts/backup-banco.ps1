<#
.SINOPSE
    Backup do banco do Instituto Tribo de Davi (SQL Server), com compactação e
    expurgo dos backups antigos.

.DESCRIÇÃO
    Gera um .bak, compacta em .zip e apaga backups com mais de N dias.
    Pensado para rodar diariamente pelo Agendador de Tarefas do Windows.

    IMPORTANTE: um backup só vale depois de testado. Restaure num banco de
    teste de tempos em tempos — ver RESTAURACAO em BACKUP.md.

.EXEMPLO
    .\backup-banco.ps1 -Servidor "localhost" -Banco "TriboDeDavi" -Destino "D:\Backups\Tribo"

.EXEMPLO
    # Autenticação SQL (quando não for Windows Auth)
    .\backup-banco.ps1 -Servidor "meu-servidor" -Banco "TriboDeDavi" -Destino "D:\Backups" -Usuario "sa" -Senha "..."
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$Servidor,
    [Parameter(Mandatory = $true)][string]$Banco,
    [Parameter(Mandatory = $true)][string]$Destino,
    [string]$Usuario,
    [string]$Senha,
    # Backups mais antigos que isso são apagados após um novo backup bem-sucedido.
    [int]$DiasRetencao = 30
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $Destino)) {
    New-Item -ItemType Directory -Force -Path $Destino | Out-Null
}

$carimbo = Get-Date -Format "yyyy-MM-dd_HHmm"
$arquivoBak = Join-Path $Destino "$Banco`_$carimbo.bak"
$arquivoZip = Join-Path $Destino "$Banco`_$carimbo.zip"
$log = Join-Path $Destino "backup.log"

function Escrever($mensagem) {
    $linha = "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')  $mensagem"
    Write-Host $linha
    Add-Content -Path $log -Value $linha -Encoding utf8
}

try {
    Escrever "Iniciando backup de [$Banco] em $Servidor"

    # COMPRESSION reduz bastante o tamanho; CHECKSUM detecta corrupção na origem.
    $sql = @"
BACKUP DATABASE [$Banco]
TO DISK = N'$arquivoBak'
WITH INIT, COMPRESSION, CHECKSUM, STATS = 10,
     NAME = N'$Banco - backup automatico';
"@

    $argumentos = @("-S", $Servidor, "-b", "-Q", $sql)
    if ($Usuario) { $argumentos += @("-U", $Usuario, "-P", $Senha) }
    else { $argumentos += "-E" }  # autenticação integrada do Windows

    & sqlcmd @argumentos
    if ($LASTEXITCODE -ne 0) { throw "sqlcmd retornou $LASTEXITCODE" }

    # Confere a integridade do arquivo antes de considerar o backup bom.
    $verificacao = @("-S", $Servidor, "-b", "-Q", "RESTORE VERIFYONLY FROM DISK = N'$arquivoBak' WITH CHECKSUM;")
    if ($Usuario) { $verificacao += @("-U", $Usuario, "-P", $Senha) } else { $verificacao += "-E" }
    & sqlcmd @verificacao
    if ($LASTEXITCODE -ne 0) { throw "VERIFYONLY falhou: backup possivelmente corrompido" }

    Compress-Archive -Path $arquivoBak -DestinationPath $arquivoZip -Force
    Remove-Item $arquivoBak -Force

    $tamanho = [math]::Round((Get-Item $arquivoZip).Length / 1MB, 2)
    Escrever "Backup concluido: $arquivoZip ($tamanho MB)"

    # Só expurga depois que o backup novo passou na verificação.
    $limite = (Get-Date).AddDays(-$DiasRetencao)
    $antigos = Get-ChildItem -Path $Destino -Filter "$Banco`_*.zip" |
               Where-Object { $_.LastWriteTime -lt $limite }
    foreach ($a in $antigos) {
        Remove-Item $a.FullName -Force
        Escrever "Removido backup antigo: $($a.Name)"
    }

    Escrever "OK"
    exit 0
}
catch {
    Escrever "ERRO: $($_.Exception.Message)"
    if (Test-Path $arquivoBak) { Remove-Item $arquivoBak -Force -ErrorAction SilentlyContinue }
    exit 1
}
