using System;
using System.Collections.Generic;
using DevMikroblog.Modules.Posts.Domain.Model;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevMikroblog.Modules.Posts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    reply_to_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_name = table.Column<string>(type: "text", nullable: true),
                    tags = table.Column<IReadOnlyList<Tag>>(type: "jsonb", nullable: true),
                    likes = table.Column<long>(type: "bigint", nullable: false),
                    replies_quantity = table.Column<long>(type: "bigint", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_posts", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_posts_tags",
                table: "posts",
                column: "tags")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "posts");
        }
    }
}
