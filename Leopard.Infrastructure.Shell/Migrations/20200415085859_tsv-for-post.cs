using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace Leopard.Infrastructure.Shell.Migrations
{
    public partial class tsvforpost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "Tsv",
                table: "Posts",
                nullable: true,
                computedColumnSql: @"
				setweight(to_tsvector('testzhcfg',coalesce(""Title"",'')), 'A')  ||
				setweight(to_tsvector('testzhcfg',coalesce(""Text"",'')), 'B') ");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Tsv",
                table: "Posts",
                column: "Tsv")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_Tsv",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Tsv",
                table: "Posts");
        }
    }
}
