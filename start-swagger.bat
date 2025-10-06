@echo off
echo ===============================================
echo  EXECUTAR SWAGGER - MODO UNIVERSAL
echo ===============================================
echo.

REM Detectar onde estamos e ir para o diretorio correto
cd /d "%~dp0"
if exist "Plataforma_Agendamentos" (
    echo Entrando no diretorio do projeto...
    cd "Plataforma_Agendamentos"
)

echo ?? Diretorio atual: %CD%
echo.

REM Verificar se estamos no lugar certo
if not exist "Program.cs" (
    echo ? ERRO: Program.cs nao encontrado!
    echo.
    echo Verifique se voce esta executando o script na pasta correta:
    echo D:\Dev_Beto\Plataforma_Agendamentos\
    echo.
    echo Ou execute diretamente:
    echo cd D:\Dev_Beto\Plataforma_Agendamentos\Plataforma_Agendamentos
    echo dotnet run
    echo.
    pause
    exit /b 1
)

echo ? Program.cs encontrado - pasta correta!
echo.

echo ?? Limpando processos e cache...
taskkill /F /IM "dotnet.exe" >nul 2>&1
if exist "bin" rmdir /s /q "bin" >nul 2>&1
if exist "obj" rmdir /s /q "obj" >nul 2>&1

echo.
echo ??? Build da aplicacao...
dotnet build --verbosity minimal

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ? ERRO NO BUILD!
    echo.
    dotnet build
    pause
    exit /b 1
)

echo ? Build OK!
echo.

echo ?? ===============================================
echo    INICIANDO PLATAFORMA DE AGENDAMENTOS
echo ===============================================
echo.
echo URLs para testar apos inicializacao:
echo.
echo ?? SWAGGER:
echo   ? https://localhost:5001/swagger (principal)
echo   ? http://localhost:5000/swagger  (se SSL falhar)
echo.
echo ?? OUTROS ENDPOINTS:
echo   ? https://localhost:5001/         (home)
echo   ? https://localhost:5001/api/health (health check)
echo.
echo ?? DICAS:
echo   - Se aparecer aviso de certificado, clique "Avancado" ? "Continuar"
echo   - Se HTTPS nao funcionar, use a versao HTTP
echo   - Aguarde a mensagem "PLATAFORMA DE AGENDAMENTOS INICIADA!"
echo.
echo ?? Para parar: Pressione Ctrl+C
echo ===============================================
echo.

set ASPNETCORE_ENVIRONMENT=Development
dotnet run --urls="https://localhost:5001;http://localhost:5000"

echo.
echo ===============================================
echo  APLICACAO ENCERRADA
echo ===============================================
pause