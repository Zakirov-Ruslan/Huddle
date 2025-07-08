using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Huddle.Channel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class added_message_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChannelId",
                schema: "Channels",
                table: "Messages",
                column: "ChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Channels_ChannelId",
                schema: "Channels",
                table: "Messages",
                column: "ChannelId",
                principalSchema: "Channels",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Channels_ChannelId",
                schema: "Channels",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChannelId",
                schema: "Channels",
                table: "Messages");
        }
    }
}
