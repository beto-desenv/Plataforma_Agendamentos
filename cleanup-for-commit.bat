@echo off
echo ================================================
echo  LIMPEZA DO WORKSPACE PARA COMMIT
echo ================================================
echo.

cd /d "%~dp0"

echo ?? Limpando arquivos temporários e de diagnóstico...
echo.

REM Arquivos de log e debug
if exist "app-debug.log" del "app-debug.log" && echo ? app-debug.log removido
if exist "logs-app.txt" del "logs-app.txt" && echo ? logs-app.txt removido
if exist "test*.txt" del "test*.txt" && echo ? Arquivos de teste removidos
if exist "test*.json" del "test*.json" && echo ? Arquivos JSON de teste removidos
if exist "test*.html" del "test*.html" && echo ? Arquivos HTML de teste removidos
if exist "swagger*.txt" del "swagger*.txt" && echo ? Arquivos de teste do Swagger removidos
if exist "swagger*.json" del "swagger*.json" && echo ? Arquivos JSON do Swagger removidos
if exist "swagger*.html" del "swagger*.html" && echo ? Arquivos HTML do Swagger removidos
if exist "health*.txt" del "health*.txt" && echo ? Arquivos de teste do Health removidos
if exist "*.tmp" del "*.tmp" && echo ? Arquivos temporários removidos

REM Pasta de logs se existir
if exist "logs" rmdir /s /q "logs" && echo ? Pasta logs removida

echo.
echo ?? Arquivos de diagnóstico a serem removidos:
echo    (Scripts temporários criados para troubleshooting)
echo.

REM Scripts de diagnóstico temporários (manter apenas os essenciais)
set "keep_scripts="
set "remove_scripts=full-diagnosis.bat quick-test-http.bat swagger-step-by-step.bat swagger-simple-mode.bat swagger-http-https-test.bat swagger-diagnosis.bat force-swagger.bat swagger-final-fix.bat fix-namespace-conflict.bat test-swagger-final.bat start-and-test.bat admin-fix.bat kill-processes.bat total-restart.bat"

for %%f in (%remove_scripts%) do (
    if exist "%%f" (
        del "%%f" && echo ? %%f removido
    )
)

echo.
echo ?? Arquivos de documentação de troubleshooting a serem removidos:
echo.

REM Documentação de troubleshooting temporária
set "remove_docs=SWAGGER-TROUBLESHOOTING.md SWAGGER-SOLUCOES-DEFINITIVAS.md SWAGGER-FIX-RAPIDO.md CORRECAO-ROTAS-FRONTEND.md PROCESSO-BLOQUEADO-SOLUCAO.md APLICACAO-NAO-INICIA.md WORKSPACE-STATUS-REPORT.md"

for %%f in (%remove_docs%) do (
    if exist "%%f" (
        del "%%f" && echo ? %%f removido
    )
)

echo.
echo ?? Limpando cache e builds temporários...

cd Plataforma_Agendamentos
if exist "bin" rmdir /s /q "bin" && echo ? bin/ removido
if exist "obj" rmdir /s /q "obj" && echo ? obj/ removido

REM Arquivos temporários do projeto
if exist "Program-*.cs" del "Program-*.cs" && echo ? Arquivos Program temporários removidos
if exist "fix-*.bat" del "fix-*.bat" && echo ? Scripts de correção temporários removidos

cd ..

echo.
echo ?? Arquivos mantidos (essenciais):
echo    ? README.md
echo    ? api-config.json (configuração para frontend)
echo    ? COMO-EXECUTAR.md (guia de execução)
echo    ? start-swagger.bat (script principal)
echo    ? test-swagger-final.ps1 (PowerShell principal)
echo    ? Todos os arquivos do projeto (.cs, .csproj, etc.)
echo    ? .gitignore
echo.

echo ?? Arquivos prontos para commit:
echo.
echo    ?? Plataforma_Agendamentos/
echo       ??? ?? Controllers/
echo       ??? ?? Models/
echo       ??? ?? DTOs/
echo       ??? ?? Services/
echo       ??? ?? Middleware/
echo       ??? ?? Validators/
echo       ??? ?? Data/
echo       ??? ?? Program.cs (corrigido)
echo       ??? ?? appsettings.json
echo       ??? ?? Plataforma_Agendamentos.csproj
echo.
echo    ?? README.md
echo    ?? api-config.json
echo    ?? COMO-EXECUTAR.md
echo    ?? start-swagger.bat
echo    ?? test-swagger-final.ps1
echo    ?? .gitignore
echo.

echo ? Limpeza concluída! Workspace pronto para commit.
echo.

pause