using FlyFramework.Core.RoleService;
using FlyFramework.Core.UserService;
using FlyFramework.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace FlyFramework.EntityFrameworkCore
{
    public class FlyFrameworkDbContext : IdentityDbContext<User, Role, string>
    {
        public FlyFrameworkDbContext(DbContextOptions<FlyFrameworkDbContext> options)
        : base(options)
        {
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            //OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            //{
            //    if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            //    {
            //        entityType.AddSoftDeleteQueryFilter();
            //    }
            //}

            //从当前程序集中加载实现了IDesignTimeDbContextFactory接口的配置类
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

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
                CreationTime = DateTime.Now
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
                CreationTime = DateTime.Now
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
                RoleId = adminRoleId
            });

        }

        public DbSet<User> User { get; set; }
    }
}
