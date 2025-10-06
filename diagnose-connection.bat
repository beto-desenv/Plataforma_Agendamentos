@echo off
echo ================================================
echo  DIAGNOSTICO DE CONEXAO - FRONTEND/BACKEND
echo ================================================
echo.

cd /d "%~dp0Plataforma_Agendamentos"

echo ?? 1. Verificando portas ocupadas...
echo.
echo Portas 5000 e 5001:
netstat -ano | findstr ":500"
echo.

echo ?? 2. Testando se PostgreSQL esta rodando...
echo.
psql --version 2>nul && echo "? PostgreSQL instalado" || echo "? PostgreSQL nao encontrado"
echo.

echo ?? 3. Compilando projeto...
echo.
dotnet build
if %ERRORLEVEL% NEQ 0 (
    echo ? Erro na compilacao
    pause
    exit /b 1
)
echo ? Compilacao OK
echo.

echo ?? 4. Iniciando API...
echo.
echo URLs que a API vai usar:
echo   - HTTPS: https://localhost:5001
echo   - HTTP:  http://localhost:5000  
echo   - Swagger: https://localhost:5001/swagger
echo   - Health: https://localhost:5001/api/health
echo.
echo IMPORTANTE: Seu frontend deve acessar https://localhost:5001/api
echo.
echo ?? Iniciando aplicacao...
echo (Pressione Ctrl+C para parar)
echo.

dotnet run --urls="https://localhost:5001;http://localhost:5000"

pause