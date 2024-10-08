using FlyFramework.Entities;
using FlyFramework.UserModule;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Wheel.EntityFrameworkCore
{
    /// <summary>
    /// EF拦截器
    /// </summary>
    public sealed class FlyFrameworkEFCoreInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            OnSavingChanges(eventData);
            return base.SavingChanges(eventData, result);
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            OnSavingChanges(eventData);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        public static void OnSavingChanges(DbContextEventData eventData)
        {
            ArgumentNullException.ThrowIfNull(eventData.Context);
            eventData.Context.ChangeTracker.DetectChanges();
            foreach (var entityEntry in eventData.Context.ChangeTracker.Entries())
            {
                if (entityEntry is { State: EntityState.Deleted, Entity: ISoftDelete softDeleteEntity })
                {
                    softDeleteEntity.IsDeleted = true;
                    entityEntry.State = EntityState.Modified;
                }
                //if (entityEntry is { State: EntityState.Modified, Entity: IHasUpdateTime hasUpdateTimeEntity })
                //{
                //    hasUpdateTimeEntity.UpdateTime = DateTimeOffset.Now;
                //}
                if (entityEntry is { State: EntityState.Added, Entity: ICreationAuditedEntity<string> hasCreationTimeEntity })
                {
                    hasCreationTimeEntity.CreationTime = DateTime.Now;
                    //entityEntry.GetType().GetProperty("Id").SetValue(entityEntry, Guid.NewGuid).ToString("N"));
                    //hasCreationTimeEntity.CreatorUserName = DateTimeOffset.Now;
                    //hasCreationTimeEntity.CreationTime = DateTimeOffset.Now;

                }
            }
        }
    }
}
