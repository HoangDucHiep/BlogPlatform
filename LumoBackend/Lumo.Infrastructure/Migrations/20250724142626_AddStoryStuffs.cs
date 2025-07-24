using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumo.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddStoryStuffs : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "stories",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                content = table.Column<string>(type: "text", nullable: true),
                author_id = table.Column<Guid>(type: "uuid", nullable: false),
                publication_id = table.Column<Guid>(type: "uuid", nullable: true),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                published_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                is_paywalled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                read_time_calculated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                last_updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_stories", x => x.id);
                table.ForeignKey(
                    name: "fk_stories_user_author_id",
                    column: x => x.author_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "save_change_versions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                story_id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                content = table.Column<string>(type: "text", nullable: false),
                description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                last_updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_save_change_versions", x => x.id);
                table.ForeignKey(
                    name: "fk_save_change_versions_story_story_id",
                    column: x => x.story_id,
                    principalTable: "stories",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_save_change_versions_created_at_utc",
            table: "save_change_versions",
            column: "created_at_utc");

        migrationBuilder.CreateIndex(
            name: "ix_save_change_versions_story_id",
            table: "save_change_versions",
            column: "story_id");

        migrationBuilder.CreateIndex(
            name: "ix_save_change_versions_story_id_created_at_utc",
            table: "save_change_versions",
            columns: ["story_id", "created_at_utc"]);

        migrationBuilder.CreateIndex(
            name: "ix_stories_author_id",
            table: "stories",
            column: "author_id");

        migrationBuilder.CreateIndex(
            name: "ix_stories_author_id_status",
            table: "stories",
            columns: ["author_id", "status"]);

        migrationBuilder.CreateIndex(
            name: "ix_stories_content",
            table: "stories",
            column: "content");

        migrationBuilder.CreateIndex(
            name: "ix_stories_published_at_utc",
            table: "stories",
            column: "published_at_utc");

        migrationBuilder.CreateIndex(
            name: "ix_stories_status",
            table: "stories",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "IX_Stories_Title",
            table: "stories",
            column: "title");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "save_change_versions");

        migrationBuilder.DropTable(
            name: "stories");
    }
}
