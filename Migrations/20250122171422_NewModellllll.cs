using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class NewModellllll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "PasswordDetails");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "PasswordChanges",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "PasswordChanges");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "PasswordDetails",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
