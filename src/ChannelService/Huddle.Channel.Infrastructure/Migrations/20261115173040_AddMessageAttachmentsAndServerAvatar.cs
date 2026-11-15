using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Huddle.Channel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageAttachmentsAndServerAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AvatarFileId",
                schema: "Channels",
                table: "Servers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "message_attachments",
                schema: "Channels",
                columns: table => new
                {
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    IsImage = table.Column<bool>(type: "boolean", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message_attachments", x => new { x.FileId, x.Position });
                    table.ForeignKey(
                        name: "FK_message_attachments_Messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "Channels",
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_message_attachments_MessageId",
                schema: "Channels",
                table: "message_attachments",
                column: "MessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "message_attachments",
                schema: "Channels");

            migrationBuilder.DropColumn(
                name: "AvatarFileId",
                schema: "Channels",
                table: "Servers");
        }
    }
}
