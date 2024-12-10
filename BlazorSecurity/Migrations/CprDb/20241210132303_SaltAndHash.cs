using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorSecurity.Migrations.CprDb
{
    /// <inheritdoc />
    public partial class SaltAndHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CprNumber",
                table: "CprUsers",
                newName: "CprNumberHash");

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "CprUsers",
                type: "BLOB",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "CprUsers");

            migrationBuilder.RenameColumn(
                name: "CprNumberHash",
                table: "CprUsers",
                newName: "CprNumber");
        }
    }
}
