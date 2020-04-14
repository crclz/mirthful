using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace Leopard.Infrastructure.Shell.Migrations
{
    public partial class discusstsvmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "TextTsv",
                table: "Discussions",
                nullable: true,
                computedColumnSql: "to_tsvector('testzhcfg',coalesce(\"Text\",'')) ",
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_TextTsv",
                table: "Discussions",
                column: "TextTsv")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Discussions_TextTsv",
                table: "Discussions");

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "TextTsv",
                table: "Discussions",
                type: "tsvector",
                nullable: true,
                oldClrType: typeof(NpgsqlTsVector),
                oldNullable: true,
                oldComputedColumnSql: "to_tsvector('testzhcfg',coalesce(\"Text\",'')) ");
        }
    }
}
