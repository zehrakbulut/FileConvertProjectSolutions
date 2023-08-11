using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileConvertProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class addFileRanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileRanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastAddress = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileRanges", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileRanges");
        }
    }
}
