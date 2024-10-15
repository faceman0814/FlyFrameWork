using FlyFramework.Entities;
using FlyFramework.UserModule;
using FlyFramework.UserSessions;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FlyFramework
{
    public abstract class DbContextBase : IdentityDbContext<User, Role, string>
    {
        public IUserSession UserSession { get; set; }

        protected DbContextBase()
        {
        }

        protected DbContextBase(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        protected virtual void BeforeSaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IEntity<string> && (
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified ||
                    e.State == EntityState.Deleted));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    ((IEntity<string>)entityEntry.Entity).Id = Guid.NewGuid().ToString("N");
                    if (entityEntry.Entity is ICreationAuditedEntity<string>)
                    {
                        ((ICreationAuditedEntity<string>)entityEntry.Entity).CreationTime = DateTime.Now;
                        ((ICreationAuditedEntity<string>)entityEntry.Entity).CreatorUserId = UserSession?.UserId;
                        ((ICreationAuditedEntity<string>)entityEntry.Entity).CreatorUserName = UserSession?.UserName;
                    }
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    if (entityEntry.Entity is IAuditedEntity<string>)
                    {
                        ((IAuditedEntity<string>)entityEntry.Entity).LastModificationTime = DateTime.Now;
                        ((IAuditedEntity<string>)entityEntry.Entity).LastModifierUserId = UserSession?.UserId;
                        ((IAuditedEntity<string>)entityEntry.Entity).LastModifierUserName = UserSession?.UserName;
                    }
                }
                else if (entityEntry.State == EntityState.Deleted)
                {
                    //如果实体为硬删除，则不执行任何操作
                    if (entityEntry.Entity is IHardDelete)
                    {
                        continue;
                    }
                    //如果实体为软删除，则设置IsDeleted为true，并设置DeletionTime、DeleterUserId、DeleterUserName
                    if (entityEntry.Entity is IFullAuditedEntity<string>)
                    {
                        entityEntry.State = EntityState.Modified;
                        ((ISoftDelete)entityEntry.Entity).IsDeleted = true;
                        ((IFullAuditedEntity<string>)entityEntry.Entity).DeletionTime = DateTime.Now;
                        ((IFullAuditedEntity<string>)entityEntry.Entity).DeleterUserId = UserSession?.UserId;
                        ((IFullAuditedEntity<string>)entityEntry.Entity).DeleterUserName = UserSession?.UserName;
                    }
                    else if (entityEntry.Entity is ISoftDelete)
                    {
                        entityEntry.State = EntityState.Modified;
                        ((ISoftDelete)entityEntry.Entity).IsDeleted = true;
                    }
                }
            }

        }

        protected virtual void AfterSaveChanges()
        {

        }

        public override int SaveChanges()
        {
            BeforeSaveChanges();
            var result = base.SaveChanges();
            AfterSaveChanges();
            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            BeforeSaveChanges();
            var result = await base.SaveChangesAsync(cancellationToken);
            AfterSaveChanges();
            return result;
        }
    }
}
