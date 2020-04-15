using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace Leopard.Infrastructure.Shell.Migrations
{
    public partial class tsv_for_reply : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "TextTsv",
                table: "Replies",
                nullable: true,
                computedColumnSql: " to_tsvector('testzhcfg',coalesce(\"Text\",'')) ");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_TextTsv",
                table: "Replies",
                column: "TextTsv")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Replies_TextTsv",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "TextTsv",
                table: "Replies");
        }
    }
}
