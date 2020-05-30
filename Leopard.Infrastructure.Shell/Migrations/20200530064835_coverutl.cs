using Microsoft.EntityFrameworkCore.Migrations;

namespace Leopard.Infrastructure.Shell.Migrations
{
    public partial class coverutl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverUrl",
                table: "Works",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverUrl",
                table: "Works");
        }
    }
}
