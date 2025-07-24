using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCentral.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameLayoutJsonToPartsJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parts");

            migrationBuilder.RenameColumn(
                name: "LayoutJson",
                table: "Builds",
                newName: "PartsJson");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PartsJson",
                table: "Builds",
                newName: "LayoutJson");

            migrationBuilder.CreateTable(
                name: "Parts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.Id);
                });
        }
    }
}
