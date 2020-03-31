using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace Leopard.Infrastructure.Shell.Migrations
{
    public partial class tsv_for_work : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "Tsv",
                table: "Works",
                nullable: true,
                computedColumnSql: @"
				setweight(to_tsvector('testzhcfg',coalesce(""Name"",'')), 'A')    ||
				setweight(to_tsvector('testzhcfg',coalesce(""Author"",'')), 'A')  ||
				setweight(to_tsvector('testzhcfg',coalesce(""Description"",'')), 'B') ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tsv",
                table: "Works");
        }
    }
}
