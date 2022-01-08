using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoRapide.Utilisateurs.API.Migrations
{
    public partial class AjoutCodeUsager : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodeUniqueUsager",
                table: "Usager",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeUniqueUsager",
                table: "Usager");
        }
    }
}
