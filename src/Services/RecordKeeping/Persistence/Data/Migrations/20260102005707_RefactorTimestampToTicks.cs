using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordBot.Service.RecordKeeping.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTimestampToTicks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Timestamp",
                table: "Messages",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(48)");

            migrationBuilder.AlterColumn<long>(
                name: "EditedTimestamp",
                table: "Messages",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(48)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Timestamp",
                table: "Messages",
                type: "nvarchar(48)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "EditedTimestamp",
                table: "Messages",
                type: "nvarchar(48)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
