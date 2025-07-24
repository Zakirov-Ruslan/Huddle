using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Huddle.Channel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class added_invite_configuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Invites_ServerId_UserId",
                schema: "Channels",
                table: "Invites",
                columns: new[] { "ServerId", "UserId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_Servers_ServerId",
                schema: "Channels",
                table: "Invites",
                column: "ServerId",
                principalSchema: "Channels",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invites_Servers_ServerId",
                schema: "Channels",
                table: "Invites");

            migrationBuilder.DropIndex(
                name: "IX_Invites_ServerId_UserId",
                schema: "Channels",
                table: "Invites");
        }
    }
}
