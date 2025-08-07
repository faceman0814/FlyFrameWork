using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlyFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeaderId",
                table: "OrgUnitNode");

            migrationBuilder.AddColumn<bool>(
                name: "IsSuperAdmin",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "是否超级管理员");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuperAdmin",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "LeaderId",
                table: "OrgUnitNode",
                type: "text",
                nullable: true,
                comment: "部门负责人ID");
        }
    }
}
