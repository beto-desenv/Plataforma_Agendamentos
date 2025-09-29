@echo off
echo ================================================
echo  TESTANDO PLATAFORMA DE AGENDAMENTOS
echo ================================================
echo.

echo ?? Parando processos existentes...
taskkill /F /IM "Plataforma_Agendamentos.exe" 2>nul
taskkill /F /IM "dotnet.exe" 2>nul

echo.
echo ?? Iniciando aplicacao...
cd /d "%~dp0Plataforma_Agendamentos"

start "" dotnet run

echo.
echo ? Aguardando 5 segundos para a aplicacao inicializar...
timeout /t 5 /nobreak >nul

echo.
echo ?? Abrindo navegador...
start "" "https://localhost:5001/"

echo.
echo ?? Abrindo Swagger...
timeout /t 2 /nobreak >nul
start "" "https://localhost:5001/swagger"

echo.
echo ================================================
echo  URLs da aplicacao:
echo    Home: https://localhost:5001/
echo    Swagger: https://localhost:5001/swagger  
echo    API: https://localhost:5001/api
echo ================================================
echo.
echo Pressione qualquer tecla para fechar...
pause >nul