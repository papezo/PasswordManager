using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AccountDetails",
                newName: "Username");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "AccountDetails",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "AccountDetails");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "AccountDetails",
                newName: "Name");
        }
    }
}
