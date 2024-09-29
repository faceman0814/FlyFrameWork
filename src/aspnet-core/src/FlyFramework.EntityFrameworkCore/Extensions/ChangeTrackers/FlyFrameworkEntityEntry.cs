using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System.Collections.Generic;
using System.Linq;

namespace FlyFramework.Extensions.ChangeTrackers;

public class FlyFrameworkEntityEntry
{
    public string Id { get; set; }

    public EntityEntry EntityEntry { get; set; }

    public List<FlyFrameworkNavigationEntry> NavigationEntries { get; set; }

    private bool _isModified;
    public bool IsModified
    {
        get
        {
            return _isModified || EntityEntry.State == EntityState.Modified || NavigationEntries.Any(n => n.IsModified);
        }
        set
        {
            _isModified = value;
        }
    }

    public FlyFrameworkEntityEntry(string id, EntityEntry entityEntry)
    {
        Id = id;
        EntityEntry = entityEntry;
        NavigationEntries = EntityEntry.Navigations.Select(x => new FlyFrameworkNavigationEntry(x, x.Metadata.Name)).ToList();
    }
}

public class FlyFrameworkNavigationEntry
{
    public NavigationEntry NavigationEntry { get; set; }

    public string Name { get; set; }

    public bool IsModified { get; set; }

    public FlyFrameworkNavigationEntry(NavigationEntry navigationEntry, string name)
    {
        NavigationEntry = navigationEntry;
        Name = name;
    }
}
