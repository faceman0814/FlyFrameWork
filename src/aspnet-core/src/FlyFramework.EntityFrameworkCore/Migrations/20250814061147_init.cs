using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FlyFramework.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: true, comment: "显示名称"),
                    IsStatic = table.Column<bool>(type: "boolean", nullable: false, comment: "是否静态角色"),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false, comment: "是否默认角色"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, comment: "是否已删除"),
                    DeleterUserId = table.Column<string>(type: "text", nullable: true, comment: "删除人Id"),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "删除时间"),
                    DeleterUserName = table.Column<string>(type: "text", nullable: true, comment: "删除人名称"),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "最后修改时间"),
                    LastModifierUserName = table.Column<string>(type: "text", nullable: true, comment: "最后修改人名称"),
                    LastModifierUserId = table.Column<string>(type: "text", nullable: true, comment: "最后修改人Id"),
                    ConcurrencyToken = table.Column<string>(type: "text", nullable: true, comment: "并发令牌"),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "创建时间"),
                    CreatorUserName = table.Column<string>(type: "text", nullable: true, comment: "创建人名称"),
                    CreatorUserId = table.Column<string>(type: "text", nullable: true, comment: "创建人Id"),
                    TenantId = table.Column<string>(type: "text", nullable: true, comment: "租户Id"),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true, comment: "用户名称"),
                    OrgUnitNodeId = table.Column<string>(type: "text", nullable: true, comment: "组织单元Id"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, comment: "是否启用"),
                    IsSuperAdmin = table.Column<bool>(type: "boolean", nullable: false, comment: "是否超级管理员"),
                    NeedToChangeThePassword = table.Column<bool>(type: "boolean", nullable: false, comment: "是否需要修改密码"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, comment: "是否已删除"),
                    DeleterUserId = table.Column<string>(type: "text", nullable: true, comment: "删除人Id"),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "删除时间"),
                    DeleterUserName = table.Column<string>(type: "text", nullable: true, comment: "删除人名称"),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "最后修改时间"),
                    LastModifierUserName = table.Column<string>(type: "text", nullable: true, comment: "最后修改人名称"),
                    LastModifierUserId = table.Column<string>(type: "text", nullable: true, comment: "最后修改人Id"),
                    ConcurrencyToken = table.Column<string>(type: "text", nullable: true, comment: "并发令牌"),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "创建时间"),
                    CreatorUserName = table.Column<string>(type: "text", nullable: true, comment: "创建人名称"),
                    CreatorUserId = table.Column<string>(type: "text", nullable: true, comment: "创建人Id"),
                    TenantId = table.Column<string>(type: "text", nullable: true, comment: "租户Id"),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrgUnitNode",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, comment: "主键"),
                    Name = table.Column<string>(type: "text", nullable: true, comment: "组织机构名称"),
                    ParentId = table.Column<string>(type: "text", nullable: true, comment: "父级Id"),
                    TenantId = table.Column<string>(type: "text", nullable: true, comment: "租户ID"),
                    Status = table.Column<int>(type: "integer", nullable: false, comment: "状态"),
                    ConcurrencyToken = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "并发令牌"),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "创建时间"),
                    CreatorUserName = table.Column<string>(type: "text", nullable: true, comment: "创建人名称"),
                    CreatorUserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "创建人Id"),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "最后修改时间"),
                    LastModifierUserName = table.Column<string>(type: "text", nullable: true, comment: "最后修改人名称"),
                    LastModifierUserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "最后修改人Id"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, comment: "是否已删除"),
                    DeleterUserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "删除人Id"),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "删除时间"),
                    DeleterUserName = table.Column<string>(type: "text", nullable: true, comment: "删除人名称")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgUnitNode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrgUnitNodeRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, comment: "主键"),
                    OrgUnitNodeId = table.Column<string>(type: "text", nullable: true, comment: "节点关联的数据Id"),
                    RoleId = table.Column<string>(type: "text", nullable: true, comment: "角色Id"),
                    TenantId = table.Column<string>(type: "text", nullable: true, comment: "租户Id"),
                    ConcurrencyToken = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "并发令牌"),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "创建时间"),
                    CreatorUserName = table.Column<string>(type: "text", nullable: true, comment: "创建人名称"),
                    CreatorUserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "创建人Id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgUnitNodeRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, comment: "主键"),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<string>(type: "character varying(32)", nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "并发令牌")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permission_Permission_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Permission",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, comment: "主键"),
                    RoleId = table.Column<string>(type: "text", nullable: true),
                    PermissionId = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "并发令牌"),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "创建时间"),
                    CreatorUserName = table.Column<string>(type: "text", nullable: true, comment: "创建人名称"),
                    CreatorUserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "创建人Id"),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "最后修改时间"),
                    LastModifierUserName = table.Column<string>(type: "text", nullable: true, comment: "最后修改人名称"),
                    LastModifierUserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "最后修改人Id"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, comment: "是否已删除"),
                    DeleterUserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "删除人Id"),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "删除时间"),
                    DeleterUserName = table.Column<string>(type: "text", nullable: true, comment: "删除人名称")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    Id = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true, comment: "是否已删除"),
                    DeleterUserId = table.Column<string>(type: "text", nullable: true, comment: "删除人Id"),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "删除时间"),
                    DeleterUserName = table.Column<string>(type: "text", nullable: true, comment: "删除人名称"),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "最后修改时间"),
                    LastModifierUserName = table.Column<string>(type: "text", nullable: true, comment: "最后修改人名称"),
                    LastModifierUserId = table.Column<string>(type: "text", nullable: true, comment: "最后修改人Id"),
                    ConcurrencyToken = table.Column<string>(type: "text", nullable: true, comment: "并发令牌"),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "创建时间"),
                    CreatorUserName = table.Column<string>(type: "text", nullable: true, comment: "创建人名称"),
                    CreatorUserId = table.Column<string>(type: "text", nullable: true, comment: "创建人Id"),
                    TenantId = table.Column<string>(type: "text", nullable: true, comment: "租户Id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permission_ParentId",
                table: "Permission",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "OrgUnitNode");

            migrationBuilder.DropTable(
                name: "OrgUnitNodeRole");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
