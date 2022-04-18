using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveShare.Migrations
{
    public partial class AddedNavigationPropBetweenUserAndUserStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DeleteData(
            //    table: "UserStatus",
            //    keyColumn: "Id",
            //    keyValue: 1);

            //migrationBuilder.DeleteData(
            //    table: "UserStatus",
            //    keyColumn: "Id",
            //    keyValue: 2);

            //migrationBuilder.DeleteData(
            //    table: "UserStatus",
            //    keyColumn: "Id",
            //    keyValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Status",
                table: "AspNetUsers",
                column: "Status",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserStatus_Status",
                table: "AspNetUsers",
                column: "Status",
                principalTable: "UserStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserStatus_Status",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Status",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AspNetUsers");

            //migrationBuilder.InsertData(
            //    table: "UserStatus",
            //    columns: new[] { "Id", "Status" },
            //    values: new object[] { 1, "Active" });

            //migrationBuilder.InsertData(
            //    table: "UserStatus",
            //    columns: new[] { "Id", "Status" },
            //    values: new object[] { 2, "Inactive" });

            //migrationBuilder.InsertData(
            //    table: "UserStatus",
            //    columns: new[] { "Id", "Status" },
            //    values: new object[] { 3, "Blocked" });
        }
    }
}
