using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Huddle.Channel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class dunno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SeverUsername",
                schema: "Channels",
                table: "Members",
                newName: "Profile_SeverUsername");

            migrationBuilder.AddColumn<string>(
                name: "Profile_Description",
                schema: "Channels",
                table: "Members",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Invites",
                schema: "Channels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServerId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invites", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invites",
                schema: "Channels");

            migrationBuilder.DropColumn(
                name: "Profile_Description",
                schema: "Channels",
                table: "Members");

            migrationBuilder.RenameColumn(
                name: "Profile_SeverUsername",
                schema: "Channels",
                table: "Members",
                newName: "SeverUsername");
        }
    }
}
