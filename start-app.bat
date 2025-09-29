@echo off
echo ================================================
echo  PLATAFORMA DE AGENDAMENTOS - INICIALIZACAO
echo ================================================
echo.

cd /d "%~dp0Plataforma_Agendamentos"

echo Restaurando pacotes...
dotnet restore

echo.
echo Compilando aplicacao...
dotnet build

echo.
echo Iniciando aplicacao...
echo.
echo URLS da aplicacao:
echo   HTTPS: https://localhost:5001
echo   HTTP:  http://localhost:5000
echo   Swagger: https://localhost:5001/swagger
echo.
echo Para parar: Ctrl+C
echo.

dotnet run

pause