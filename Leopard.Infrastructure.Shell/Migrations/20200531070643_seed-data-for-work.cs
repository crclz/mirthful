using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Leopard.Infrastructure.Shell.Migrations
{
    public partial class seeddataforwork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Works",
                columns: new[] { "Id", "Author", "CoverUrl", "CreatedAt", "Description", "Name", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0a180b32-2510-406f-b0fd-1e19b6bb2697"), "some author", "https://i.loli.net/2020/05/31/prCLIHej56ZMOwo.jpg", 1590908802534L, "description hello", "testbook1", 0, 1590908802534L },
                    { new Guid("71a710dc-aab6-4b9e-97eb-61ae1d703117"), "some director", "https://i.loli.net/2020/05/31/prCLIHej56ZMOwo.jpg", 1590908802542L, "description hello", "testfilm1", 1, 1590908802542L }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Works",
                keyColumn: "Id",
                keyValue: new Guid("0a180b32-2510-406f-b0fd-1e19b6bb2697"));

            migrationBuilder.DeleteData(
                table: "Works",
                keyColumn: "Id",
                keyValue: new Guid("71a710dc-aab6-4b9e-97eb-61ae1d703117"));
        }
    }
}
