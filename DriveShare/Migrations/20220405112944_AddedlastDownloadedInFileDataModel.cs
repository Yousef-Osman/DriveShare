using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveShare.Migrations
{
    public partial class AddedlastDownloadedInFileDataModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastDownloaded",
                table: "Files",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastDownloaded",
                table: "Files");
        }
    }
}
