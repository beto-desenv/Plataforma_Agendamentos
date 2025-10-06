@echo off
echo ===============================================
echo  TESTANDO API COM MELHORIAS IMPLEMENTADAS
echo ===============================================
echo.

cd /d "%~dp0Plataforma_Agendamentos"

echo ?? Parando processos existentes...
taskkill /F /IM "Plataforma_Agendamentos.exe" 2>nul
taskkill /F /IM "dotnet.exe" /FI "WINDOWTITLE eq *Plataforma_Agendamentos*" 2>nul

echo.
echo ?? Verificando pre-requisitos...
echo.

REM Verificar se PostgreSQL esta rodando
pg_ctl status 2>nul && echo "? PostgreSQL rodando" || echo "?? PostgreSQL pode nao estar rodando"

REM Verificar se as portas estao livres
netstat -ano | findstr ":5001" >nul && echo "?? Porta 5001 esta ocupada" || echo "? Porta 5001 livre"
netstat -ano | findstr ":5000" >nul && echo "?? Porta 5000 esta ocupada" || echo "? Porta 5000 livre"

echo.
echo ??? Compilando projeto...
dotnet build --no-restore
if %ERRORLEVEL% NEQ 0 (
    echo ? Falha na compilacao
    pause
    exit /b 1
)
echo ? Compilacao bem-sucedida

echo.
echo ?? Iniciando aplicacao com logging aprimorado...
echo.
echo ?? Recursos implementados:
echo   ? Logging estruturado com Serilog
echo   ? Middleware de tratamento de erros global  
echo   ? Validação com FluentValidation
echo   ? Health checks (/api/health)
echo   ? Request/Response logging
echo   ? Security headers
echo   ? Documentação aprimorada
echo.
echo ?? URLs para testar:
echo   ?? Home: https://localhost:5001/
echo   ?? Swagger: https://localhost:5001/swagger
echo   ?? Health Check: https://localhost:5001/api/health
echo   ?? Health Info: https://localhost:5001/api/health/info
echo   ?? Ping: https://localhost:5001/api/health/ping
echo.
echo ?? Endpoints de autenticacao:
echo   ?? Registro: POST https://localhost:5001/api/auth/register
echo   ?? Login: POST https://localhost:5001/api/auth/login  
echo   ?? Perfil: GET https://localhost:5001/api/auth/profile
echo.
echo ?? IMPORTANTE: Configure seu frontend para usar https://localhost:5001/api
echo.
echo Pressione Ctrl+C para parar a aplicacao...
echo.

REM Iniciar com URLs explícitas
dotnet run --urls="https://localhost:5001;http://localhost:5000"

echo.
echo ?? Aplicacao encerrada
pause