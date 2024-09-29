using JetBrains.Annotations;

using System;

namespace FlyFramework.TenantModule;

public interface ICurrentTenant
{
    bool IsAvailable { get; }

    Guid? Id { get; }

    string Name { get; }

    IDisposable Change(Guid? id, string name = null);
}
