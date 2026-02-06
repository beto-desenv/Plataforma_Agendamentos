using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plataforma_Agendamentos.Migrations
{
    /// <inheritdoc />
    public partial class RefactorUserToSeparateProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Slug",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AceitaAgendamentoImediato",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AnosExperiencia",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AvaliacaoMedia",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CNPJ",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CPF",
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
                name: "CoverImageUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DataNascimento",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EnderecoCliente",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EnderecoPrestador",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EstadoCliente",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EstadoPrestador",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HorarioFimSemana",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HorarioInicioSemana",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HorasAntecedenciaMinima",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InteressesServicos",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PreferenciasNotificacao",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PrimaryColor",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RaioAtendimento",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Site",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TelefoneCliente",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TelefonePrestador",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TituloProfissional",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalAgendamentosCliente",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalAgendamentosPrestador",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalAvaliacoes",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalServicos",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UltimoAgendamento",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "ClientePerfis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ContatoPreferido = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CPF = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Endereco = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CEP = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    PreferenciasNotificacao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    InteressesServicos = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TotalAgendamentos = table.Column<int>(type: "integer", nullable: false),
                    UltimoAgendamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientePerfis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientePerfis_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrestadorPerfis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TituloProfissional = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Bio = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CPF = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    CNPJ = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    AnosExperiencia = table.Column<int>(type: "integer", nullable: true),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Site = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Endereco = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CEP = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    RaioAtendimento = table.Column<int>(type: "integer", nullable: true),
                    AceitaAgendamentoImediato = table.Column<bool>(type: "boolean", nullable: false),
                    HorasAntecedenciaMinima = table.Column<int>(type: "integer", nullable: false),
                    HorarioInicioSemana = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    HorarioFimSemana = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrestadorPerfis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrestadorPerfis_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrestadorBrandings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PrestadorPerfilId = table.Column<Guid>(type: "uuid", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "text", nullable: true),
                    PrimaryColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrestadorBrandings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrestadorBrandings_PrestadorPerfis_PrestadorPerfilId",
                        column: x => x.PrestadorPerfilId,
                        principalTable: "PrestadorPerfis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrestadorMetricas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PrestadorPerfilId = table.Column<Guid>(type: "uuid", nullable: false),
                    AvaliacaoMedia = table.Column<decimal>(type: "numeric(3,2)", nullable: false),
                    TotalAvaliacoes = table.Column<int>(type: "integer", nullable: false),
                    TotalServicos = table.Column<int>(type: "integer", nullable: false),
                    TotalAgendamentos = table.Column<int>(type: "integer", nullable: false),
                    UltimaAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrestadorMetricas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrestadorMetricas_PrestadorPerfis_PrestadorPerfilId",
                        column: x => x.PrestadorPerfilId,
                        principalTable: "PrestadorPerfis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientePerfis_CPF",
                table: "ClientePerfis",
                column: "CPF");

            migrationBuilder.CreateIndex(
                name: "IX_ClientePerfis_UserId",
                table: "ClientePerfis",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrestadorBrandings_PrestadorPerfilId",
                table: "PrestadorBrandings",
                column: "PrestadorPerfilId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrestadorMetricas_PrestadorPerfilId",
                table: "PrestadorMetricas",
                column: "PrestadorPerfilId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrestadorPerfis_CNPJ",
                table: "PrestadorPerfis",
                column: "CNPJ");

            migrationBuilder.CreateIndex(
                name: "IX_PrestadorPerfis_Slug",
                table: "PrestadorPerfis",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrestadorPerfis_UserId",
                table: "PrestadorPerfis",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientePerfis");

            migrationBuilder.DropTable(
                name: "PrestadorBrandings");

            migrationBuilder.DropTable(
                name: "PrestadorMetricas");

            migrationBuilder.DropTable(
                name: "PrestadorPerfis");

            migrationBuilder.AddColumn<bool>(
                name: "AceitaAgendamentoImediato",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AnosExperiencia",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AvaliacaoMedia",
                table: "Users",
                type: "numeric(3,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Users",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CNPJ",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CPF",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
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
                name: "CoverImageUrl",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataNascimento",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnderecoCliente",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnderecoPrestador",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
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
                name: "HorarioFimSemana",
                table: "Users",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HorarioInicioSemana",
                table: "Users",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HorasAntecedenciaMinima",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InteressesServicos",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferenciasNotificacao",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryColor",
                table: "Users",
                type: "character varying(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RaioAtendimento",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Site",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefoneCliente",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefonePrestador",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TituloProfissional",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalAgendamentosCliente",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalAgendamentosPrestador",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalAvaliacoes",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalServicos",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimoAgendamento",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Slug",
                table: "Users",
                column: "Slug",
                unique: true);
        }
    }
}
