using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Extensions
{
    public static class EntityEntryExtensions
    {
        //
        // 摘要:
        //     Check if the entity and its associated Owned entity have changed.
        //
        // 参数:
        //   entry:
        public static bool CheckOwnedEntityChange(this EntityEntry entry)
        {
            if (entry.State != EntityState.Modified)
            {
                return entry.References.Any((ReferenceEntry r) => r.TargetEntry != null && r.TargetEntry.Metadata.IsOwned() && r.TargetEntry.CheckOwnedEntityChange());
            }

            return true;
        }
    }
}
