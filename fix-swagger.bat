@echo off
echo ===============================================
echo  CORRIGINDO SWAGGER - RESTART COMPLETO
echo ===============================================
echo.

cd /d "%~dp0Plataforma_Agendamentos"

echo ?? Parando TODOS os processos dotnet e da aplicacao...
taskkill /F /IM "Plataforma_Agendamentos.exe" 2>nul
taskkill /F /IM "dotnet.exe" 2>nul
timeout /t 2 /nobreak >nul

echo ?? Limpando arquivos de build...
if exist "bin" rmdir /s /q "bin" 2>nul
if exist "obj" rmdir /s /q "obj" 2>nul
if exist "logs" rmdir /s /q "logs" 2>nul

echo ?? Restaurando pacotes...
dotnet restore

echo ??? Build completo...
dotnet build

echo.
echo ?? Iniciando aplicacao com Swagger CORRIGIDO...
echo.
echo ? URLs para testar apos inicializacao:
echo   ?? Swagger: https://localhost:5001/swagger
echo   ?? Home: https://localhost:5001/
echo   ?? Health: https://localhost:5001/api/health
echo   ?? Ping: https://localhost:5001/api/health/ping
echo.
echo ?? Aguarde a mensagem: "PLATAFORMA DE AGENDAMENTOS INICIADA!"
echo.

set ASPNETCORE_ENVIRONMENT=Development
dotnet run --urls="https://localhost:5001;http://localhost:5000"

echo.
echo ?? Aplicacao encerrada
pause