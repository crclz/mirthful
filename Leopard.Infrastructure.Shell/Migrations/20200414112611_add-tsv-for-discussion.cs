using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace Leopard.Infrastructure.Shell.Migrations
{
    public partial class addtsvfordiscussion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "TextTsv",
                table: "Discussions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextTsv",
                table: "Discussions");
        }
    }
}
