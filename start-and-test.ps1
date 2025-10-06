# ================================================
# INICIAR APLICAÇÃO E TESTAR ROTAS (PowerShell)
# ================================================

Write-Host "================================================" -ForegroundColor Cyan
Write-Host " INICIAR APLICAÇÃO E TESTAR ROTAS" -ForegroundColor Yellow  
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Ir para o diretório correto
Set-Location $PSScriptRoot
if (Test-Path "Plataforma_Agendamentos") {
    Set-Location "Plataforma_Agendamentos"
}

Write-Host "?? Verificando se aplicação já está rodando..." -ForegroundColor Cyan
$portCheck = netstat -ano | Select-String ":5001"
if ($portCheck) {
    Write-Host "? Aplicação já está rodando na porta 5001" -ForegroundColor Green
} else {
    Write-Host "?? Matando processos existentes..." -ForegroundColor Red
    Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force

    Write-Host ""
    Write-Host "?? Limpando cache..." -ForegroundColor Cyan
    if (Test-Path "bin") { Remove-Item -Path "bin" -Recurse -Force -ErrorAction SilentlyContinue }
    if (Test-Path "obj") { Remove-Item -Path "obj" -Recurse -Force -ErrorAction SilentlyContinue }

    Write-Host ""
    Write-Host "??? Fazendo build..." -ForegroundColor Cyan
    $buildResult = & dotnet build --verbosity quiet 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? ERRO NO BUILD - Aplicação não pode iniciar" -ForegroundColor Red
        & dotnet build
        Read-Host "Pressione Enter para sair"
        exit 1
    }

    Write-Host "? Build OK" -ForegroundColor Green
    Write-Host ""

    Write-Host "?? Iniciando aplicação..." -ForegroundColor Cyan
    Write-Host "?? Aguarde 20 segundos para inicialização completa..." -ForegroundColor Yellow

    # Iniciar aplicação usando Start-Process em vez de Start-Job para melhor controle
    $processInfo = Start-Process -FilePath "dotnet" -ArgumentList "run", "--urls=https://localhost:5001;http://localhost:5000" -PassThru -WindowStyle Hidden
    
    # Aguardar inicialização
    Start-Sleep -Seconds 20
}

Write-Host ""
Write-Host "?? Testando endpoints..." -ForegroundColor Cyan
Write-Host ""

# Função para testar endpoint
function Test-Endpoint {
    param($Url, $Name, $ExpectedText)
    
    try {
        $response = Invoke-RestMethod -Uri $Url -SkipCertificateCheck -TimeoutSec 10 -ErrorAction Stop
        $responseText = $response | ConvertTo-Json -Compress
        
        if ($responseText -like "*$ExpectedText*") {
            Write-Host "? $Name`: $Url - OK" -ForegroundColor Green
            return $true
        } else {
            Write-Host "? $Name`: Conecta mas resposta inválida" -ForegroundColor Red
            Write-Host "Resposta: $($responseText.Substring(0, [Math]::Min(100, $responseText.Length)))" -ForegroundColor Gray
            return $false
        }
    }
    catch {
        Write-Host "? $Name`: $Url - NÃO CONECTA" -ForegroundColor Red
        Write-Host "Erro: $($_.Exception.Message)" -ForegroundColor Gray
        return $false
    }
}

# Testar endpoints
Write-Host "?? Testando HOME..." -ForegroundColor Cyan
$homeResult = Test-Endpoint "https://localhost:5001/" "HOME" "Message"

Write-Host "?? Testando HEALTH..." -ForegroundColor Cyan  
$healthResult = Test-Endpoint "https://localhost:5001/api/health" "HEALTH" "status"

Write-Host "?? Testando PING..." -ForegroundColor Cyan
$pingResult = Test-Endpoint "https://localhost:5001/api/health/ping" "PING" "OK"

Write-Host "?? Testando SWAGGER JSON..." -ForegroundColor Cyan
$swaggerJsonResult = Test-Endpoint "https://localhost:5001/swagger/v1/swagger.json" "SWAGGER JSON" "openapi"

Write-Host "?? Testando SWAGGER UI..." -ForegroundColor Cyan
try {
    $swaggerResponse = Invoke-WebRequest -Uri "https://localhost:5001/swagger" -SkipCertificateCheck -TimeoutSec 10 -UseBasicParsing
    if ($swaggerResponse.Content -like "*swagger-ui*") {
        Write-Host "? SWAGGER UI: https://localhost:5001/swagger - OK" -ForegroundColor Green
        $swaggerUIResult = $true
    } else {
        Write-Host "? SWAGGER UI: HTML inválido" -ForegroundColor Red
        $swaggerUIResult = $false
    }
}
catch {
    Write-Host "? SWAGGER UI: https://localhost:5001/swagger - NÃO CONECTA" -ForegroundColor Red
    $swaggerUIResult = $false
}

Write-Host ""
Write-Host "?? RESUMO DOS TESTES:" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan

$workingCount = @($homeResult, $healthResult, $pingResult, $swaggerJsonResult, $swaggerUIResult) | Where-Object { $_ -eq $true } | Measure-Object | Select-Object -ExpandProperty Count

if ($workingCount -gt 0) {
    if ($homeResult) { Write-Host "? Home funcionando" -ForegroundColor Green }
    if ($healthResult) { Write-Host "? Health funcionando" -ForegroundColor Green }
    if ($pingResult) { Write-Host "? Ping funcionando" -ForegroundColor Green }
    if ($swaggerJsonResult) { Write-Host "? Swagger JSON funcionando" -ForegroundColor Green }
    if ($swaggerUIResult) { Write-Host "? Swagger UI funcionando" -ForegroundColor Green }
    
    Write-Host ""
    Write-Host "?? Abrindo URLs funcionais no navegador..." -ForegroundColor Cyan
    
    if ($homeResult) { 
        Start-Process "https://localhost:5001/"
        Start-Sleep -Seconds 2
    }
    if ($swaggerUIResult) { 
        Start-Process "https://localhost:5001/swagger"
        Start-Sleep -Seconds 2
    }
    if ($healthResult) { 
        Start-Process "https://localhost:5001/api/health"
    }
    
    Write-Host ""
    Write-Host "? Aplicação funcionando! URLs abertas no navegador." -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "? APLICAÇÃO NÃO ESTÁ RESPONDENDO!" -ForegroundColor Red
    Write-Host ""
    Write-Host "?? Possíveis causas:" -ForegroundColor Yellow
    Write-Host "  1. Aplicação falhou ao iniciar" -ForegroundColor White
    Write-Host "  2. Porta ocupada por outro processo" -ForegroundColor White
    Write-Host "  3. Certificado SSL inválido" -ForegroundColor White
    Write-Host "  4. Firewall bloqueando conexões" -ForegroundColor White
    Write-Host "  5. PostgreSQL não está rodando" -ForegroundColor White
    Write-Host ""
    Write-Host "?? Tente executar: .\full-diagnosis.bat para diagnóstico completo" -ForegroundColor Cyan
    Write-Host ""
}

Write-Host "?? Se HTTPS não funcionar, tente HTTP: http://localhost:5000/swagger" -ForegroundColor Yellow
Write-Host ""
Write-Host "Pressione qualquer tecla para MANTER a aplicação rodando..." -ForegroundColor Yellow
Write-Host "(Ou feche esta janela para manter rodando em background)" -ForegroundColor Gray
Read-Host

if ($processInfo -and !$processInfo.HasExited) {
    Write-Host ""
    Write-Host "?? Parando aplicação..." -ForegroundColor Red
    Stop-Process -Id $processInfo.Id -ErrorAction SilentlyContinue
    Write-Host "? Aplicação parada" -ForegroundColor Green
}

Write-Host ""
Write-Host "Script finalizado." -ForegroundColor Gray