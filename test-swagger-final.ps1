# ================================================
# TESTE FINAL - SWAGGER FUNCIONANDO (PowerShell)
# ================================================

Write-Host "================================================" -ForegroundColor Cyan
Write-Host " TESTE FINAL - SWAGGER FUNCIONANDO" -ForegroundColor Yellow
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Mudar para o diretório correto
Set-Location $PSScriptRoot
if (Test-Path "Plataforma_Agendamentos") {
    Set-Location "Plataforma_Agendamentos"
}

Write-Host "?? Matando processos existentes..." -ForegroundColor Red
try {
    Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force
    Write-Host "? Processos dotnet finalizados" -ForegroundColor Green
} catch {
    Write-Host "?? Nenhum processo dotnet encontrado" -ForegroundColor Gray
}

try {
    Get-Process -Name "Plataforma_Agendamentos" -ErrorAction SilentlyContinue | Stop-Process -Force
    Write-Host "? Processos da aplicação finalizados" -ForegroundColor Green
} catch {
    Write-Host "?? Nenhum processo da aplicação encontrado" -ForegroundColor Gray
}

Write-Host ""
Write-Host "?? Verificando portas..." -ForegroundColor Cyan

$port5001 = netstat -ano | findstr ":5001"
$port5000 = netstat -ano | findstr ":5000"

if ($port5001) {
    Write-Host "?? Porta 5001 ocupada" -ForegroundColor Yellow
} else {
    Write-Host "? Porta 5001 livre" -ForegroundColor Green
}

if ($port5000) {
    Write-Host "?? Porta 5000 ocupada" -ForegroundColor Yellow
} else {
    Write-Host "? Porta 5000 livre" -ForegroundColor Green
}

Write-Host ""
Write-Host "?? Limpando cache de build..." -ForegroundColor Cyan

if (Test-Path "bin") {
    Remove-Item -Path "bin" -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "? Diretório bin removido" -ForegroundColor Green
}

if (Test-Path "obj") {
    Remove-Item -Path "obj" -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "? Diretório obj removido" -ForegroundColor Green
}

Write-Host ""
Write-Host "??? Build limpo..." -ForegroundColor Cyan

& dotnet clean --verbosity quiet
& dotnet restore --verbosity quiet
$buildResult = & dotnet build --verbosity quiet

if ($LASTEXITCODE -ne 0) {
    Write-Host "? ERRO NO BUILD - Verifique os erros acima" -ForegroundColor Red
    Read-Host "Pressione Enter para sair"
    exit 1
}

Write-Host "? Build bem-sucedido!" -ForegroundColor Green

Write-Host ""
Write-Host "?? Iniciando aplicação..." -ForegroundColor Cyan
Write-Host ""
Write-Host "?? Aguarde aparecer 'PLATAFORMA DE AGENDAMENTOS INICIADA!'" -ForegroundColor Yellow
Write-Host "Depois teste estas URLs:" -ForegroundColor White
Write-Host ""
Write-Host "  ?? Swagger HTTPS: https://localhost:5001/swagger" -ForegroundColor White
Write-Host "  ?? Swagger HTTP:  http://localhost:5000/swagger" -ForegroundColor White
Write-Host "  ?? Home: https://localhost:5001/" -ForegroundColor White
Write-Host "  ?? Health: https://localhost:5001/api/health" -ForegroundColor White
Write-Host ""
Write-Host "?? Se HTTPS não funcionar por causa do certificado, use HTTP" -ForegroundColor Yellow
Write-Host ""

$env:ASPNETCORE_ENVIRONMENT = "Development"
& dotnet run --urls="https://localhost:5001;http://localhost:5000"

Write-Host ""
Write-Host "?? Aplicação encerrada" -ForegroundColor Gray
Read-Host "Pressione Enter para finalizar"