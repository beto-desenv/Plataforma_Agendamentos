@echo off
echo ================================================
echo  RECRIANDO BANCO DE DADOS E MIGRATIONS
echo ================================================
echo.

cd /d "%~dp0Plataforma_Agendamentos"

echo ??? Removendo banco de dados atual...
echo DROP DATABASE IF EXISTS plataforma_agendamentos_dev; CREATE DATABASE plataforma_agendamentos_dev; | psql -U plataforma_user -h localhost

echo.
echo ?? Limpando migrations...
if exist "Migrations" rmdir /s /q "Migrations"

echo.
echo ?? Criando migration inicial...
dotnet ef migrations add InitialCreate

echo.
echo ?? Aplicando migrations ao banco...
dotnet ef database update

echo.
echo ? Banco recriado com sucesso!
echo.
echo ?? Iniciando aplicacao...
dotnet run

pause