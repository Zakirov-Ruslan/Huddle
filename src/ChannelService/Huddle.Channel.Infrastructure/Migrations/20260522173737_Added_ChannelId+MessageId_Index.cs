using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Huddle.Channel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_ChannelIdMessageId_Index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_ChannelId",
                schema: "Channels",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChannelId_Id",
                schema: "Channels",
                table: "Messages",
                columns: new[] { "ChannelId", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_ChannelId_Id",
                schema: "Channels",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChannelId",
                schema: "Channels",
                table: "Messages",
                column: "ChannelId");
        }
    }
}
