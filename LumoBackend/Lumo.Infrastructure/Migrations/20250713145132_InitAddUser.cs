using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumo.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitAddUser : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                identity_id = table.Column<string>(type: "text", nullable: false),
                user_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                email_address = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                bio = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                profile_picture_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                cover_picture_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                social_links = table.Column<string>(type: "jsonb", nullable: true),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                last_updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_users_email_address",
            table: "users",
            column: "email_address",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_identity_id",
            table: "users",
            column: "identity_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_user_name",
            table: "users",
            column: "user_name");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "users");
    }
}
