using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveShare.Migrations
{
    public partial class AddedIsPrivateInFileDataModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Files",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Files");
        }
    }
}
