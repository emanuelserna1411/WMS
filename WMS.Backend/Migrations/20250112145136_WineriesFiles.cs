using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.Backend.Migrations
{
    /// <inheritdoc />
    public partial class WineriesFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "File1",
                table: "Wineries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "File2",
                table: "Wineries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "File3",
                table: "Wineries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "File4",
                table: "Wineries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "File5",
                table: "Wineries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "File1",
                table: "Wineries");

            migrationBuilder.DropColumn(
                name: "File2",
                table: "Wineries");

            migrationBuilder.DropColumn(
                name: "File3",
                table: "Wineries");

            migrationBuilder.DropColumn(
                name: "File4",
                table: "Wineries");

            migrationBuilder.DropColumn(
                name: "File5",
                table: "Wineries");
        }
    }
}
