using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordBot.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MessageReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "ReferencedMessageId",
                table: "Messages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReferencedMessageId",
                table: "Messages",
                column: "ReferencedMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_ReferencedMessageId",
                table: "Messages",
                column: "ReferencedMessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_ReferencedMessageId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ReferencedMessageId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ReferencedMessageId",
                table: "Messages");
        }
    }
}
