using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class NewModellll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewValue",
                table: "PasswordChanges");

            migrationBuilder.DropColumn(
                name: "OldValue",
                table: "PasswordChanges");

            migrationBuilder.AddColumn<int>(
                name: "logId",
                table: "PasswordChanges",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "logId",
                table: "PasswordChanges");

            migrationBuilder.AddColumn<string>(
                name: "NewValue",
                table: "PasswordChanges",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldValue",
                table: "PasswordChanges",
                type: "TEXT",
                nullable: true);
        }
    }
}
