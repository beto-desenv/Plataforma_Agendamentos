using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plataforma_Agendamentos.Migrations
{
    /// <inheritdoc />
    public partial class AddNewUserFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Users_ProviderId",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Services",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "ProviderId",
                table: "Services",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Services",
                newName: "Preco");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ProviderId",
                table: "Services",
                newName: "IX_Services_UserId");

            migrationBuilder.AddColumn<int>(
                name: "AnosExperiencia",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CPFPrestador",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CidadeCliente",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CidadePrestador",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContatoPreferido",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoCliente",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoPrestador",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoPerfilUrl",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InteressesServicos",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RaioAtendimento",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TituloProfissional",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AtualizadoEm",
                table: "Services",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now() at time zone 'UTC'");

            migrationBuilder.AddColumn<DateTime>(
                name: "CriadoEm",
                table: "Services",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now() at time zone 'UTC'");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Users_UserId",
                table: "Services",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Users_UserId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "AnosExperiencia",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CPFPrestador",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CidadeCliente",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CidadePrestador",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ContatoPreferido",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EstadoCliente",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EstadoPrestador",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FotoPerfilUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InteressesServicos",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RaioAtendimento",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TituloProfissional",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AtualizadoEm",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CriadoEm",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Services",
                newName: "ProviderId");

            migrationBuilder.RenameColumn(
                name: "Preco",
                table: "Services",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Services",
                newName: "Title");

            migrationBuilder.RenameIndex(
                name: "IX_Services_UserId",
                table: "Services",
                newName: "IX_Services_ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Users_ProviderId",
                table: "Services",
                column: "ProviderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
