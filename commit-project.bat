@echo off
echo ================================================
echo  COMMIT ORGANIZADO - PLATAFORMA FUNCIONANDO
echo ================================================
echo.

cd /d "%~dp0"

echo ?? Executando limpeza primeiro...
call cleanup-for-commit.bat
echo.

echo ?? Verificando status do Git...
git status
echo.

echo ?? Preparando commit estruturado...
echo.

echo ?? O que será commitado:
echo.
echo ? BACKEND COMPLETO (.NET 8):
echo    - API REST funcionando 100%%
echo    - Autenticação JWT implementada
echo    - CRUD completo (Users, Services, Schedules, Bookings)
echo    - PostgreSQL integrado
echo    - Swagger UI operacional
echo    - Health checks configurados
echo    - Middleware de logging e tratamento de erros
echo    - Validação com FluentValidation
echo    - Segurança implementada (BCrypt, CORS, Headers)
echo.
echo ? DOCUMENTAÇÃO ATUALIZADA:
echo    - README.md completo
echo    - api-config.json para frontend
echo    - COMO-EXECUTAR.md com guias
echo    - Scripts de execução (start-swagger.bat, test-swagger-final.ps1)
echo.
echo ? ESTRUTURA LIMPA:
echo    - Arquivos de troubleshooting removidos
echo    - Cache e builds temporários limpos
echo    - Apenas código essencial mantido
echo.

echo.
set /p confirm="Confirma o commit? (s/n): "
if /i "%confirm%" neq "s" (
    echo Commit cancelado.
    pause
    exit /b 0
)

echo.
echo ?? Adicionando arquivos ao Git...
git add .

echo.
echo ?? Fazendo commit...
git commit -m "feat: API completa funcionando - Plataforma de Agendamentos

? Features implementadas:
- API REST completa com .NET 8
- Autenticação JWT + BCrypt
- CRUD completo: Users, Services, Schedules, Bookings
- PostgreSQL com Entity Framework
- Swagger UI documentação interativa
- Health checks + monitoramento
- Middleware logging e error handling
- FluentValidation para validação
- CORS e security headers
- Perfis públicos para prestadores

?? Endpoints funcionais:
- POST /api/auth/register - Registro usuários
- POST /api/auth/login - Autenticação
- GET /api/auth/profile - Perfil usuário
- CRUD /api/services - Gestão serviços
- CRUD /api/schedules - Horários disponíveis
- CRUD /api/bookings - Sistema reservas
- GET /api/prestador/{slug} - Perfil público
- GET /api/health - Monitoramento

?? Documentação:
- README.md atualizado com guia completo
- api-config.json para integração frontend
- Scripts execução: start-swagger.bat, test-swagger-final.ps1
- Guia COMO-EXECUTAR.md

?? URLs funcionais:
- https://localhost:5001/swagger - Swagger UI
- https://localhost:5001/api/health - Health check
- https://localhost:5001/ - API info

Ready para desenvolvimento frontend! ??"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ? COMMIT REALIZADO COM SUCESSO!
    echo.
    echo ?? Plataforma de Agendamentos - Backend completo commitado!
    echo.
    echo ?? Próximos passos recomendados:
    echo    1. git push origin develop
    echo    2. Criar pull request para main
    echo    3. Iniciar desenvolvimento do frontend
    echo    4. Usar api-config.json como base
    echo.
    echo ?? URLs para testar:
    echo    - Swagger: https://localhost:5001/swagger
    echo    - Health: https://localhost:5001/api/health
    echo    - Home: https://localhost:5001/
    echo.
    
    set /p push="Deseja fazer push agora? (s/n): "
    if /i "%push%" equ "s" (
        echo.
        echo ?? Fazendo push para origin develop...
        git push origin develop
        
        if %ERRORLEVEL% EQU 0 (
            echo ? PUSH REALIZADO COM SUCESSO!
            echo.
            echo ?? Repositório atualizado em:
            echo    https://github.com/beto-desenv/Plataforma_Agendamentos
            echo.
        ) else (
            echo ? Erro no push. Verifique conexão e permissões.
        )
    )
    
) else (
    echo ? Erro no commit. Verifique os arquivos e tente novamente.
)

echo.
echo Script finalizado.
pause