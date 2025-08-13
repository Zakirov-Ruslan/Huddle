using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Huddle.Channel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Invite_Config_Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invites_ServerId_UserId",
                schema: "Channels",
                table: "Invites");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Channels",
                table: "Invites");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "Channels",
                table: "Invites",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_Code",
                schema: "Channels",
                table: "Invites",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invites_ServerId",
                schema: "Channels",
                table: "Invites",
                column: "ServerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invites_Code",
                schema: "Channels",
                table: "Invites");

            migrationBuilder.DropIndex(
                name: "IX_Invites_ServerId",
                schema: "Channels",
                table: "Invites");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "Channels",
                table: "Invites");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "Channels",
                table: "Invites",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Invites_ServerId_UserId",
                schema: "Channels",
                table: "Invites",
                columns: new[] { "ServerId", "UserId" },
                unique: true);
        }
    }
}
