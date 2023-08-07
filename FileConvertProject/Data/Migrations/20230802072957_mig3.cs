using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileConvertProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "fileConverts");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "fileConverts");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "fileConverts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "fileConverts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "fileConverts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "fileConverts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
