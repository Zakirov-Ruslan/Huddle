using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Huddle.Channel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Invite_Config_AddedUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_ServerId_IdentityId",
                schema: "Channels",
                table: "Members");

            migrationBuilder.CreateIndex(
                name: "IX_Members_ServerId_IdentityId",
                schema: "Channels",
                table: "Members",
                columns: new[] { "ServerId", "IdentityId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_ServerId_IdentityId",
                schema: "Channels",
                table: "Members");

            migrationBuilder.CreateIndex(
                name: "IX_Members_ServerId_IdentityId",
                schema: "Channels",
                table: "Members",
                columns: new[] { "ServerId", "IdentityId" });
        }
    }
}
