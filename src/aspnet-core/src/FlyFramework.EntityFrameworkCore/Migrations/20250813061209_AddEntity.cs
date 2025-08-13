using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlyFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PermissionId",
                table: "Permission",
                type: "character varying(32)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permission_PermissionId",
                table: "Permission",
                column: "PermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_Permission_PermissionId",
                table: "Permission",
                column: "PermissionId",
                principalTable: "Permission",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permission_Permission_PermissionId",
                table: "Permission");

            migrationBuilder.DropIndex(
                name: "IX_Permission_PermissionId",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "PermissionId",
                table: "Permission");
        }
    }
}
