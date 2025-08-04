using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumo.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddSlugForStory : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "slug",
            table: "stories",
            type: "character varying(300)",
            maxLength: 300,
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateIndex(
            name: "IX_Stories_Slug",
            table: "stories",
            column: "slug",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Stories_Slug",
            table: "stories");

        migrationBuilder.DropColumn(
            name: "slug",
            table: "stories");
    }
}
