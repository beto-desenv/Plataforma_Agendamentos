@echo off
echo ===============================================
echo  INICIO RAPIDO - PLATAFORMA DE AGENDAMENTOS
echo ===============================================
echo.

cd /d "%~dp0Plataforma_Agendamentos"

echo ?? Iniciando API em modo rapido...
echo.
echo URLs:
echo   ?? API: https://localhost:5001/api
echo   ?? Swagger: https://localhost:5001/swagger
echo   ?? Health: https://localhost:5001/api/health
echo.

REM Iniciar direto sem verificações
dotnet run --urls="https://localhost:5001;http://localhost:5000"

pause