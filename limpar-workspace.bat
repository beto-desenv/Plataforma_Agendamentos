@echo off
echo ================================================
echo  ANALISE DE ARQUIVOS - O QUE MANTER/REMOVER
echo ================================================
echo.

cd /d "%~dp0"

echo ?? ANALISANDO ARQUIVOS DESNECESSARIOS...
echo.

echo ? ARQUIVOS DE TROUBLESHOOTING TEMPORARIOS (REMOVER):
echo    ??? CORRECAO-ROTAS-FRONTEND.md
echo    ??? PROCESSO-BLOQUEADO-SOLUCAO.md  
echo    ??? SWAGGER-TROUBLESHOOTING.md
echo    ??? SWAGGER-SOLUCOES-DEFINITIVAS.md
echo    ??? SWAGGER-FIX-RAPIDO.md
echo    ??? WORKSPACE-STATUS-REPORT.md
echo    ??? APLICACAO-NAO-INICIA.md
echo    ??? TROUBLESHOOTING.md
echo.

echo ? SCRIPTS DE DIAGNOSTICO TEMPORARIOS (REMOVER):
echo    ??? admin-fix.bat
echo    ??? total-restart.bat
echo    ??? kill-processes.bat
echo    ??? fix-swagger.bat
echo    ??? force-swagger.bat
echo    ??? swagger-diagnosis.bat
echo    ??? swagger-step-by-step.bat
echo    ??? swagger-simple-mode.bat
echo    ??? swagger-http-https-test.bat
echo    ??? fix-namespace-conflict.bat
echo    ??? swagger-final-fix.bat
echo    ??? test-swagger-final.bat
echo    ??? start-and-test.bat
echo    ??? start-and-test.ps1
echo    ??? full-diagnosis.bat
echo    ??? quick-test-http.bat
echo    ??? test-api-routes.bat
echo    ??? quick-start.bat
echo    ??? diagnose-connection.bat
echo    ??? test-improvements.bat
echo    ??? recreate-clean.bat
echo    ??? recreate-database.bat
echo    ??? test-app.bat
echo    ??? start-app.bat
echo.

echo ? SCRIPTS SQL TEMPORARIOS (REMOVER):
echo    ??? mark_migration_as_applied.sql
echo    ??? convert_usertype_to_usertypes.sql
echo.

echo ? ARQUIVOS DE LIMPEZA (REMOVER APOS USO):
echo    ??? cleanup-for-commit.bat
echo    ??? commit-project.bat
echo.

echo ? ARQUIVOS ESSENCIAIS (MANTER):
echo.
echo ?? CORE DO PROJETO:
echo    ??? ?? Plataforma_Agendamentos/ (TODO O CONTEUDO)
echo    ?   ??? Controllers/
echo    ?   ??? Models/
echo    ?   ??? DTOs/
echo    ?   ??? Services/
echo    ?   ??? Middleware/
echo    ?   ??? Validators/
echo    ?   ??? Data/
echo    ?   ??? Properties/
echo    ?   ??? Program.cs
echo    ?   ??? *.csproj
echo.
echo ?? DOCUMENTACAO ESSENCIAL:
echo    ??? README.md (PRINCIPAL)
echo    ??? COMO-EXECUTAR.md (GUIA)
echo    ??? PROXIMOS-PASSOS.md (ROADMAP)
echo    ??? .gitignore
echo.
echo ?? CONFIGURACAO:
echo    ??? api-config.json (FRONTEND)
echo    ??? postman_collection.json (TESTES)
echo.
echo ?? SCRIPTS PRINCIPAIS:
echo    ??? start-swagger.bat (WINDOWS)
echo    ??? test-swagger-final.ps1 (POWERSHELL)
echo.

echo ?? RESUMO:
echo ? Manter: 8 arquivos + pasta do projeto
echo ? Remover: ~35 arquivos temporarios
echo ?? Economia: ~85%% dos arquivos
echo.

set /p confirm="Deseja executar a limpeza automática? (s/n): "
if /i "%confirm%" neq "s" (
    echo Limpeza cancelada.
    pause
    exit /b 0
)

echo.
echo ?? EXECUTANDO LIMPEZA...

REM Documentação de troubleshooting
del "CORRECAO-ROTAS-FRONTEND.md" 2>nul && echo ? CORRECAO-ROTAS-FRONTEND.md
del "PROCESSO-BLOQUEADO-SOLUCAO.md" 2>nul && echo ? PROCESSO-BLOQUEADO-SOLUCAO.md
del "SWAGGER-TROUBLESHOOTING.md" 2>nul && echo ? SWAGGER-TROUBLESHOOTING.md
del "SWAGGER-SOLUCOES-DEFINITIVAS.md" 2>nul && echo ? SWAGGER-SOLUCOES-DEFINITIVAS.md
del "SWAGGER-FIX-RAPIDO.md" 2>nul && echo ? SWAGGER-FIX-RAPIDO.md
del "WORKSPACE-STATUS-REPORT.md" 2>nul && echo ? WORKSPACE-STATUS-REPORT.md
del "APLICACAO-NAO-INICIA.md" 2>nul && echo ? APLICACAO-NAO-INICIA.md
del "TROUBLESHOOTING.md" 2>nul && echo ? TROUBLESHOOTING.md

REM Scripts de diagnóstico
del "admin-fix.bat" 2>nul && echo ? admin-fix.bat
del "total-restart.bat" 2>nul && echo ? total-restart.bat
del "kill-processes.bat" 2>nul && echo ? kill-processes.bat
del "fix-swagger.bat" 2>nul && echo ? fix-swagger.bat
del "force-swagger.bat" 2>nul && echo ? force-swagger.bat
del "swagger-diagnosis.bat" 2>nul && echo ? swagger-diagnosis.bat
del "swagger-step-by-step.bat" 2>nul && echo ? swagger-step-by-step.bat
del "swagger-simple-mode.bat" 2>nul && echo ? swagger-simple-mode.bat
del "swagger-http-https-test.bat" 2>nul && echo ? swagger-http-https-test.bat
del "fix-namespace-conflict.bat" 2>nul && echo ? fix-namespace-conflict.bat
del "swagger-final-fix.bat" 2>nul && echo ? swagger-final-fix.bat
del "test-swagger-final.bat" 2>nul && echo ? test-swagger-final.bat
del "start-and-test.bat" 2>nul && echo ? start-and-test.bat
del "start-and-test.ps1" 2>nul && echo ? start-and-test.ps1
del "full-diagnosis.bat" 2>nul && echo ? full-diagnosis.bat
del "quick-test-http.bat" 2>nul && echo ? quick-test-http.bat
del "test-api-routes.bat" 2>nul && echo ? test-api-routes.bat
del "quick-start.bat" 2>nul && echo ? quick-start.bat
del "diagnose-connection.bat" 2>nul && echo ? diagnose-connection.bat
del "test-improvements.bat" 2>nul && echo ? test-improvements.bat
del "recreate-clean.bat" 2>nul && echo ? recreate-clean.bat
del "recreate-database.bat" 2>nul && echo ? recreate-database.bat
del "test-app.bat" 2>nul && echo ? test-app.bat
del "start-app.bat" 2>nul && echo ? start-app.bat

REM Scripts SQL temporários
del "mark_migration_as_applied.sql" 2>nul && echo ? mark_migration_as_applied.sql
del "convert_usertype_to_usertypes.sql" 2>nul && echo ? convert_usertype_to_usertypes.sql

REM Scripts de limpeza (após uso)
del "cleanup-for-commit.bat" 2>nul && echo ? cleanup-for-commit.bat
del "commit-project.bat" 2>nul && echo ? commit-project.bat

echo.
echo ?? ESTRUTURA FINAL LIMPA:
echo.
dir /b *.md
echo.
dir /b *.bat
echo.
dir /b *.ps1
echo.
dir /b *.json
echo.

echo ? LIMPEZA CONCLUIDA!
echo.
echo ?? ARQUIVOS MANTIDOS:
echo    ? README.md - Documentação principal
echo    ? COMO-EXECUTAR.md - Guia de uso
echo    ? PROXIMOS-PASSOS.md - Roadmap
echo    ? start-swagger.bat - Script principal Windows
echo    ? test-swagger-final.ps1 - Script principal PowerShell
echo    ? api-config.json - Configuração frontend
echo    ? postman_collection.json - Collection de testes
echo    ? .gitignore - Configuração Git
echo    ? Plataforma_Agendamentos/ - Todo o projeto .NET
echo.
echo ?? Economia: ~35 arquivos removidos
echo ?? Workspace limpo e profissional!
echo.

pause