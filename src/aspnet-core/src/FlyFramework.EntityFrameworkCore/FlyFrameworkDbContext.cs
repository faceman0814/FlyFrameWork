using FlyFramework.Datas;
using FlyFramework.Entities;
using FlyFramework.Extensions.ChangeTrackers;
using FlyFramework.Extentions;
using FlyFramework.Extentions.Object;
using FlyFramework.LazyModule.LazyDefinition;
using FlyFramework.TenantModule;
using FlyFramework.UserModule;
using FlyFramework.UserSessions;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace FlyFramework
{
    public class FlyFrameworkDbContext : IdentityDbContext<User, Role, string>
    {
        public IFlyFrameworkLazy FlyFrameworkLazy { get; set; } = default!;

        public IUserSession UserSession { get; set; } = default!;
        public ILogger<FlyFrameworkDbContext> Logger => FlyFrameworkLazy.LazyGetService<ILogger<FlyFrameworkDbContext>>(NullLogger<FlyFrameworkDbContext>.Instance);

        public FlyFrameworkEfCoreNavigationHelper EfCoreNavigationHelper => new FlyFrameworkEfCoreNavigationHelper();
        //FlyFrameworkLazy.LazyGetRequiredService<FlyFrameworkEfCoreNavigationHelper>();
        public FlyFrameworkDbContext(DbContextOptions<FlyFrameworkDbContext> options)
        : base(options)
        {
        }
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            try
            {
                //获取所有变更的实体
                foreach (var entityEntry in ChangeTracker.Entries())
                {
                    switch (entityEntry.State)
                    {
                        case EntityState.Added:
                            //如果是创建实体，则设置创建人信息
                            if (entityEntry.Entity is ICreationAuditedEntity<string> creationAuditedEntity)
                            {
                                FlyFrameworkObjectExtensions.TrySetProperty(creationAuditedEntity, x => x.CreatorUserId, () => UserSession.UserId);
                                FlyFrameworkObjectExtensions.TrySetProperty(creationAuditedEntity, x => x.CreationTime, () => DateTime.Now);
                                FlyFrameworkObjectExtensions.TrySetProperty(creationAuditedEntity, x => x.CreatorUserName, () => UserSession.UserName);
                                FlyFrameworkObjectExtensions.TrySetProperty(creationAuditedEntity, x => x.ConcurrencyToken, () => Guid.NewGuid().ToString("N"));
                            }
                            break;
                        case EntityState.Modified:
                            //如果是修改实体，则设置修改人信息
                            if (entityEntry.Entity is IAuditedEntity<string> modificationAuditedEntity)
                            {
                                FlyFrameworkObjectExtensions.TrySetProperty(modificationAuditedEntity, x => x.LastModifierUserId, () => UserSession.UserId);
                                FlyFrameworkObjectExtensions.TrySetProperty(modificationAuditedEntity, x => x.LastModificationTime, () => DateTime.Now);
                                FlyFrameworkObjectExtensions.TrySetProperty(modificationAuditedEntity, x => x.LastModifierUserName, () => UserSession.UserName);
                                FlyFrameworkObjectExtensions.TrySetProperty(modificationAuditedEntity, x => x.ConcurrencyToken, () => Guid.NewGuid().ToString("N"));
                            }
                            break;
                        case EntityState.Deleted:
                            //如果是删除实体，则设置删除人信息,并将实体状态设置为修改
                            if (entityEntry.Entity is IFullAuditedEntity<string> deletionAuditedEntity)
                            {
                                FlyFrameworkObjectExtensions.TrySetProperty(deletionAuditedEntity, x => x.DeleterUserId, () => UserSession.UserId);
                                FlyFrameworkObjectExtensions.TrySetProperty(deletionAuditedEntity, x => x.DeletionTime, () => DateTime.Now);
                                FlyFrameworkObjectExtensions.TrySetProperty(deletionAuditedEntity, x => x.DeleterUserName, () => UserSession.UserName);
                                FlyFrameworkObjectExtensions.TrySetProperty(deletionAuditedEntity, x => x.IsDeleted, () => true);
                                FlyFrameworkObjectExtensions.TrySetProperty(deletionAuditedEntity, x => x.ConcurrencyToken, () => Guid.NewGuid().ToString("N"));
                                entityEntry.State = EntityState.Modified;
                            }
                            //如果是软删除实体，则设置软删除标志,并将实体状态设置为修改
                            else if (entityEntry.Entity is ISoftDelete softDeleteEntity)
                            {
                                FlyFrameworkObjectExtensions.TrySetProperty(softDeleteEntity, x => x.IsDeleted, () => true);
                                entityEntry.State = EntityState.Modified;
                            }
                            break;
                    }
                }

                var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (ex.Entries.Count > 0)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(ex.Entries.Count > 1
                        ? "There are some entries which are not saved due to concurrency exception:"
                        : "There is an entry which is not saved due to concurrency exception:");
                    foreach (var entry in ex.Entries)
                    {
                        sb.AppendLine(entry.ToString());
                    }

                    Logger.LogWarning(sb.ToString());
                }

                throw new Exception(ex.Message, ex);
            }
            finally
            {
                ChangeTracker.AutoDetectChangesEnabled = true;
                EfCoreNavigationHelper.RemoveChangedEntityEntries();
            }
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
        public DbSet<Role> Role { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
    }
}
