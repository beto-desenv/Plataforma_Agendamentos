@echo off
echo ================================================
echo  RECRIANDO BANCO COM MIGRATIONS LIMPAS
echo ================================================
echo.

cd /d "%~dp0Plataforma_Agendamentos"

echo ??? Removendo banco atual...
dotnet ef database drop --force

echo.
echo ?? Criando migration inicial limpa...
dotnet ef migrations add InitialCreate

echo.
echo ?? Criando e populando banco via migration...
dotnet ef database update

echo.
echo ? Banco recriado com sucesso via migrations!
echo.
echo ?? Iniciando aplicacao...
dotnet run

pause