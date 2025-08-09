using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Huddle.Channel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_sentAt_index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Messages_SentAt",
                schema: "Channels",
                table: "Messages",
                column: "SentAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_SentAt",
                schema: "Channels",
                table: "Messages");
        }
    }
}
