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
        public IUserSession UserSession { get; set; }

        /// <summary>
        /// 是否自动设置TenantId
        /// </summary>
        public bool SuppressAutoSetTenantId { get; set; } = false;

        protected virtual void ApplyConcepts(EntityEntry entry)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ApplyConceptsForAddedEntity(entry);
                    break;
                case EntityState.Modified:
                    //ApplyConceptsForModifiedEntity(entry);
                    break;
                case EntityState.Deleted:
                    //ApplyConceptsForDeletedEntity(entry);
                    break;
            }

        }
        protected virtual void ApplyConceptsForAddedEntity(EntityEntry entry)
        {
            CheckAndSetId(entry);
            CheckAndSetMustHaveTenantIdProperty(entry.Entity);
            CheckAndSetMayHaveTenantIdProperty(entry.Entity);
            SetCreationAuditProperties(entry.Entity);
        }

        protected virtual void CheckAndSetId(EntityEntry entry)
        {
            if (entry.Entity is IEntity<string> entity && entity.Id.IsNullOrEmpty())
            {
                PropertyEntry propertyEntry = entry.Property("Id");
                if (propertyEntry != null && propertyEntry.Metadata.ValueGenerated == ValueGenerated.Never)
                {
                    ObjectHelper.TrySetProperty(entity, x => x.Id, () => Guid.NewGuid().ToString("N"));

                }
            }
        }

        protected virtual void CheckAndSetMustHaveTenantIdProperty(object entityAsObj)
        {
            if (SuppressAutoSetTenantId || !(entityAsObj is IMustHaveTenant))
            {
                return;
            }

            IMustHaveTenant mustHaveTenant = entityAsObj.As<IMustHaveTenant>();
            if (!mustHaveTenant.TenantId.HasValue())
            {
                if (!UserSession.TenantId.HasValue())
                {
                    throw new Exception("Can not set TenantId to 0 for IMustHaveTenant entities!");
                }

                mustHaveTenant.TenantId = UserSession.TenantId;
            }
        }

        protected virtual void CheckAndSetMayHaveTenantIdProperty(object entityAsObj)
        {
            if (SuppressAutoSetTenantId || !(entityAsObj is IMayHaveTenant))
            {
                return;
            }

            IMayHaveTenant mayHaveTenant = entityAsObj.As<IMayHaveTenant>();
            if (!mayHaveTenant.TenantId.HasValue())
            {
                mayHaveTenant.TenantId = UserSession.TenantId;
            }
        }

        protected virtual void SetCreationAuditProperties(object entityAsObj)
        {
            if (entityAsObj is ICreationAuditedEntity<string> creationAuditedEntity)
            {
                if (creationAuditedEntity.CreatorUserId == null)
                {
                    creationAuditedEntity.CreatorUserId = UserSession.UserId;
                }
                if (creationAuditedEntity.CreationTime == null)
                {
                    creationAuditedEntity.CreationTime = DateTime.Now;
                }
                if (creationAuditedEntity.CreatorUserName == null)
                {
                    creationAuditedEntity.CreatorUserName = UserSession.UserName;
                }
            }
        }

        private void ConfigurationOnBeforeSaving()
        {
            foreach (EntityEntry item in ChangeTracker.Entries().ToList())
            {
                //if (item.State != EntityState.Modified && item.CheckOwnedEntityChange())
                //{
                //    Entry(item.Entity).State = EntityState.Modified;
                //}
                //ApplyConcepts(item);
                if (item is { Entity: IEntity<string> safeDelete, State: EntityState.Added })
                {
                    safeDelete.Id = Guid.NewGuid().ToString("N");
                }
            }
        }
        //protected virtual void SetModificationAuditProperties(object entityAsObj, string userId)
        //{
        //    EntityAuditingHelper.SetModificationAuditProperties(MultiTenancyConfig, entityAsObj, AbpSession.TenantId, userId, CurrentUnitOfWorkProvider?.Current?.AuditFieldConfiguration);
        //}

        //protected virtual void CancelDeletionForSoftDelete(EntityEntry entry)
        //{
        //    if (entry.Entity is ISoftDelete)
        //    {
        //        entry.Reload();
        //        entry.State = EntityState.Modified;
        //        entry.Entity.As<ISoftDelete>().IsDeleted = true;
        //    }
        //}

        //protected virtual void SetDeletionAuditProperties(object entityAsObj, string userId)
        //{
        //    EntityAuditingHelper.SetDeletionAuditProperties(MultiTenancyConfig, entityAsObj, AbpSession.TenantId, userId, CurrentUnitOfWorkProvider?.Current?.AuditFieldConfiguration);
        //}

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
