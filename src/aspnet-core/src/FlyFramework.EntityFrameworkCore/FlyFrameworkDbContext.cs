using FlyFramework.Entities;
using FlyFramework.Extentions;
using FlyFramework.Extentions.Object;
using FlyFramework.UserModule;
using FlyFramework.UserSessions;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

using System;
using System.Linq;
namespace FlyFramework
{
    public class FlyFrameworkDbContext : DbContextBase
    {
        public FlyFrameworkDbContext(DbContextOptions<FlyFrameworkDbContext> options)
        : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //从当前程序集中加载实现了IDesignTimeDbContextFactory接口的配置类
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            //设置表名
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<UserRole>().ToTable("UserRole");

            //指定键关系
            modelBuilder.Entity<User>().HasMany<UserRole>().WithOne().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Role>().HasMany<UserRole>().WithOne().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);

            //初始化数据
            //1.添加角色
            var adminRoleId = Guid.NewGuid().ToString("N");
            modelBuilder.Entity<Role>().HasData(new Role
            {
                Id = adminRoleId,
                Name = "管理员",
                CreationTime = DateTime.Now,
                TenantId = "1"
            });

            //2.添加用户
            var adminUserId = "4957adb8870f4e79882231537ff5d3b9";
            User adminUser = new User
            {
                Id = adminUserId,
                UserName = "admin",
                NormalizedUserName = "admin".ToUpper(),
                FullName = "xxx",
                PhoneNumber = "1234567890",
                Email = "1234567890@qq.com",
                NormalizedEmail = "1234567890@qq.com".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                PhoneNumberConfirmed = false,
                SecurityStamp = "Mecca",
                CreationTime = DateTime.Now,
                TenantId = "1"
            };
            PasswordHasher<User> ph = new PasswordHasher<User>();
            adminUser.PasswordHash = ph.HashPassword(adminUser, "bb123456");
            modelBuilder.Entity<User>().HasData(adminUser);

            //3.给用户加管理员身份
            var UserRoleId = Guid.NewGuid().ToString("N");
            modelBuilder.Entity<UserRole>().HasData(new UserRole
            {
                Id = UserRoleId,
                UserId = adminUserId,
                RoleId = adminRoleId,
                TenantId = "1"
            });

        }

        public DbSet<User> User { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<Role> Role { get; set; }
    }
}
