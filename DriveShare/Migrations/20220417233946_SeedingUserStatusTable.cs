using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveShare.Migrations
{
    public partial class SeedingUserStatusTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserStatus",
                columns: new[] { "Id", "Status" },
                values: new object[] { 1, "Active" });

            migrationBuilder.InsertData(
                table: "UserStatus",
                columns: new[] { "Id", "Status" },
                values: new object[] { 2, "Inactive" });

            migrationBuilder.InsertData(
                table: "UserStatus",
                columns: new[] { "Id", "Status" },
                values: new object[] { 3, "Blocked" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserStatus",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserStatus",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserStatus",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
