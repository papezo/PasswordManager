using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class Notes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "PasswordDetails",
                newName: "OneTimePassword");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "PasswordDetails",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "PasswordDetails");

            migrationBuilder.RenameColumn(
                name: "OneTimePassword",
                table: "PasswordDetails",
                newName: "Description");
        }
    }
}
