@echo off
echo ================================================
echo  TESTANDO ROTAS DA API - DIAGNOSTICO
echo ================================================
echo.

echo ?? Testando endpoints disponiveis...
echo.

echo ?? 1. Testando HOME (/)...
curl -k -s "https://localhost:5001/" | findstr "Message" >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo ? HOME: https://localhost:5001/ - OK
) else (
    echo ? HOME: https://localhost:5001/ - FALHA
)

echo.
echo ?? 2. Testando HEALTH CORRETO (/api/health)...
curl -k -s "https://localhost:5001/api/health" | findstr "status" >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo ? HEALTH: https://localhost:5001/api/health - OK
) else (
    echo ? HEALTH: https://localhost:5001/api/health - FALHA
)

echo.
echo ?? 3. Testando HEALTH PING (/api/health/ping)...
curl -k -s "https://localhost:5001/api/health/ping" | findstr "OK" >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo ? PING: https://localhost:5001/api/health/ping - OK
) else (
    echo ? PING: https://localhost:5001/api/health/ping - FALHA
)

echo.
echo ?? 4. Testando SWAGGER JSON (/swagger/v1/swagger.json)...
curl -k -s "https://localhost:5001/swagger/v1/swagger.json" | findstr "openapi" >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo ? SWAGGER JSON: https://localhost:5001/swagger/v1/swagger.json - OK
) else (
    echo ? SWAGGER JSON: https://localhost:5001/swagger/v1/swagger.json - FALHA
)

echo.
echo ?? 5. Testando SWAGGER UI (/swagger)...
curl -k -s "https://localhost:5001/swagger" | findstr "swagger-ui" >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo ? SWAGGER UI: https://localhost:5001/swagger - OK
) else (
    echo ? SWAGGER UI: https://localhost:5001/swagger - FALHA
)

echo.
echo ?? URLs CORRETAS para testar no navegador:
echo.
echo   ?? Home: https://localhost:5001/
echo   ?? Swagger: https://localhost:5001/swagger
echo   ?? Health: https://localhost:5001/api/health
echo   ?? Ping: https://localhost:5001/api/health/ping
echo   ?? Health Info: https://localhost:5001/api/health/info
echo.
echo ?? IMPORTANTE: O frontend estava tentando /api/api/health (ERRADO)
echo    Deve ser: /api/health (CORRETO)
echo.

echo ?? Abrindo URLs corretas no navegador...
start "" "https://localhost:5001/"
timeout /t 2 /nobreak >nul
start "" "https://localhost:5001/swagger"
timeout /t 2 /nobreak >nul
start "" "https://localhost:5001/api/health"

echo.
echo ? URLs abertas no navegador!
echo ?? Se aparecer aviso de certificado, clique "Avancado" ? "Continuar"
echo.

pause