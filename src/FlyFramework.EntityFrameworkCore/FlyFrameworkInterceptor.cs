using FlyFramework.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FlyFramework.EntityFrameworkCore
{
    public class FlyFrameworkInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateTimestamps(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateTimestamps(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateTimestamps(DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries())
            {
                var entityType = entry.Entity.GetType();
                var baseType = entityType.BaseType;
                if (baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(FullAuditedEntity<>))
                {
                    if (entry.State == EntityState.Added)
                    {
                        var idProperty = baseType.GetProperty(nameof(FullAuditedEntity<string>.Id));
                        if (idProperty.GetValue(entry.Entity) == null)
                        {
                            idProperty.SetValue(entry.Entity, Guid.NewGuid().ToString("N"));
                        }
                        var createdProperty = baseType.GetProperty(nameof(FullAuditedEntity<string>.CreationTime));
                        createdProperty.SetValue(entry.Entity, DateTime.UtcNow);
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        var updatedProperty = baseType.GetProperty(nameof(FullAuditedEntity<string>.LastModificationTime));
                        updatedProperty.SetValue(entry.Entity, DateTime.UtcNow);
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        var deletedProperty = baseType.GetProperty(nameof(FullAuditedEntity<string>.DeletionTime));
                        deletedProperty.SetValue(entry.Entity, DateTime.UtcNow);
                        var isDeletedProperty = baseType.GetProperty(nameof(FullAuditedEntity<bool>.IsDeleted));
                        isDeletedProperty.SetValue(entry.Entity, true);
                    }
                }
            }
        }
    }
}
