using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Leopard.Infrastructure.Shell.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<long>(nullable: false),
                    UpdatedAt = table.Column<long>(nullable: false),
                    TopicId = table.Column<Guid>(nullable: false),
                    SenderId = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attitudes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<long>(nullable: false),
                    UpdatedAt = table.Column<long>(nullable: false),
                    SenderId = table.Column<Guid>(nullable: false),
                    CommentId = table.Column<Guid>(nullable: false),
                    Agree = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attitudes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<long>(nullable: false),
                    UpdatedAt = table.Column<long>(nullable: false),
                    SenderId = table.Column<Guid>(nullable: false),
                    WorkId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    AgreeCount = table.Column<int>(nullable: false),
                    DisagreeCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeduplicationTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EventId = table.Column<Guid>(nullable: false),
                    HandlerType = table.Column<string>(nullable: true),
                    Additional = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeduplicationTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<long>(nullable: false),
                    UpdatedAt = table.Column<long>(nullable: false),
                    SenderId = table.Column<Guid>(nullable: false),
                    TopicId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    IsPinned = table.Column<bool>(nullable: false),
                    IsEssence = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Replies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<long>(nullable: false),
                    UpdatedAt = table.Column<long>(nullable: false),
                    SenderId = table.Column<Guid>(nullable: false),
                    PostId = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Replies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<long>(nullable: false),
                    UpdatedAt = table.Column<long>(nullable: false),
                    SenderId = table.Column<Guid>(nullable: false),
                    CommentId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Handled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TopicMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<long>(nullable: false),
                    UpdatedAt = table.Column<long>(nullable: false),
                    TopicId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Role = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<long>(nullable: false),
                    UpdatedAt = table.Column<long>(nullable: false),
                    IsGroup = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    RelatedWork = table.Column<Guid>(nullable: true),
                    MemberCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<long>(nullable: false),
                    UpdatedAt = table.Column<long>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Nickname = table.Column<string>(nullable: true),
                    Salt = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityVersion = table.Column<int>(nullable: false),
                    Avatar = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Works",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<long>(nullable: false),
                    UpdatedAt = table.Column<long>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Author = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Works", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeduplicationTokens_EventId_HandlerType_Additional",
                table: "DeduplicationTokens",
                columns: new[] { "EventId", "HandlerType", "Additional" },
                unique: true);

            // Text search engine
            migrationBuilder.Sql("CREATE EXTENSION zhparser;");
            migrationBuilder.Sql("CREATE TEXT SEARCH CONFIGURATION testzhcfg (PARSER = zhparser);");
            migrationBuilder.Sql("ALTER TEXT SEARCH CONFIGURATION testzhcfg ADD MAPPING FOR n,v,a,i,e,l WITH simple;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminRequests");

            migrationBuilder.DropTable(
                name: "Attitudes");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "DeduplicationTokens");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Replies");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "TopicMembers");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Works");
        }
    }
}
