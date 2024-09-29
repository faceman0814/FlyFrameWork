using System.Threading;

namespace FlyFramework.TenantModule;

public class CurrentTenantAccessor : ICurrentTenantAccessor
{
    public static CurrentTenantAccessor Instance { get; } = new();

    public BasicTenantInfo Current
    {
        get => _currentScope.Value;
        set => _currentScope.Value = value;
    }

    private readonly AsyncLocal<BasicTenantInfo> _currentScope;

    private CurrentTenantAccessor()
    {
        _currentScope = new AsyncLocal<BasicTenantInfo>();
    }
}
