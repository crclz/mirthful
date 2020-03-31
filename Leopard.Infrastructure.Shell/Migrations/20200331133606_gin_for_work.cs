using Microsoft.EntityFrameworkCore.Migrations;

namespace Leopard.Infrastructure.Shell.Migrations
{
    public partial class gin_for_work : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Works_Tsv",
                table: "Works",
                column: "Tsv")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Works_Tsv",
                table: "Works");
        }
    }
}
