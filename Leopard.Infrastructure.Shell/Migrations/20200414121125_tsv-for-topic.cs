using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace Leopard.Infrastructure.Shell.Migrations
{
    public partial class tsvfortopic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "Tsv",
                table: "Topics",
                nullable: true,
                computedColumnSql: @"
				setweight(to_tsvector('testzhcfg',coalesce(""Name"",'')), 'A')    ||
				setweight(to_tsvector('testzhcfg',coalesce(""Description"",'')), 'B') ");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Tsv",
                table: "Topics",
                column: "Tsv")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Topics_Tsv",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "Tsv",
                table: "Topics");
        }
    }
}
