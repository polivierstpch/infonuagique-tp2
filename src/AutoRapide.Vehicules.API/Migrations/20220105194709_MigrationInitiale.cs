using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoRapide.Vehicules.API.Migrations
{
    public partial class MigrationInitiale : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vehicule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Constructeur = table.Column<string>(type: "TEXT", nullable: false),
                    Modele = table.Column<string>(type: "TEXT", nullable: false),
                    AnneeFabrication = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    NombreSiege = table.Column<int>(type: "INTEGER", nullable: false),
                    Couleur = table.Column<string>(type: "TEXT", nullable: false),
                    NIV = table.Column<string>(type: "TEXT", nullable: false),
                    Image1Url = table.Column<string>(type: "TEXT", nullable: false),
                    Image2Url = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    EstDisponible = table.Column<bool>(type: "INTEGER", nullable: false),
                    Prix = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicule", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vehicule");
        }
    }
}
