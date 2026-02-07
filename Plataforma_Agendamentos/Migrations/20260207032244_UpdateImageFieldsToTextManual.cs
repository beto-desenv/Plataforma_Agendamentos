using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plataforma_Agendamentos.Migrations
{
    ///  <inheritdoc />
    public partial class UpdateImageFieldsToTextManual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Alterar FotoPerfilUrl na tabela Users
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" 
                ALTER COLUMN ""FotoPerfilUrl"" TYPE text;
            ");

            // Alterar  LogoUrl na tabela PrestadorBrandings
            migrationBuilder.Sql(@"
                ALTER TABLE ""PrestadorBrandings"" 
                ALTER COLUMN ""LogoUrl"" TYPE text;
            ");

            // Alterar CoverImageUrl na tabela PrestadorBrandings
            migrationBuilder.Sql(@"
                ALTER TABLE ""PrestadorBrandings"" 
                ALTER COLUMN ""CoverImageUrl"" TYPE text;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverter para varchar(1000)
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" 
                ALTER COLUMN ""FotoPerfilUrl"" TYPE character varying(1000);
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE ""PrestadorBrandings"" 
                ALTER COLUMN ""LogoUrl"" TYPE character varying(1000);
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE ""PrestadorBrandings"" 
                ALTER COLUMN ""CoverImageUrl"" TYPE character varying(1000);
            ");
        }
    }
}
