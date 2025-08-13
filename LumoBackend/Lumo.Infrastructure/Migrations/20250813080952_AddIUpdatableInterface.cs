using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumo.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddIUpdatableInterface : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "last_updated_at_utc",
            table: "save_change_versions");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "last_updated_at_utc",
            table: "save_change_versions",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
    }
}
