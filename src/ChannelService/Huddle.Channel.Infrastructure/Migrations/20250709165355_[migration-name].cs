using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Huddle.Channel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migrationname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerId",
                schema: "Channels",
                table: "Servers",
                newName: "OwnerIdentityId");

            migrationBuilder.RenameColumn(
                name: "IdentityGuid",
                schema: "Channels",
                table: "Members",
                newName: "SeverUsername");

            migrationBuilder.AddColumn<Guid>(
                name: "IdentityId",
                schema: "Channels",
                table: "Members",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ServerId",
                schema: "Channels",
                table: "Members",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Members_ServerId",
                schema: "Channels",
                table: "Members",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Servers_ServerId",
                schema: "Channels",
                table: "Members",
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
                name: "FK_Members_Servers_ServerId",
                schema: "Channels",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_ServerId",
                schema: "Channels",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "IdentityId",
                schema: "Channels",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ServerId",
                schema: "Channels",
                table: "Members");

            migrationBuilder.RenameColumn(
                name: "OwnerIdentityId",
                schema: "Channels",
                table: "Servers",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "SeverUsername",
                schema: "Channels",
                table: "Members",
                newName: "IdentityGuid");
        }
    }
}
