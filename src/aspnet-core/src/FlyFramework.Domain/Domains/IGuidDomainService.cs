using FlyFramework.Dependencys;
using FlyFramework.Entities;
namespace FlyFramework.Domains
{
    public interface IGuidDomainService<TEntity> : IDomainService<TEntity, string>, ITransientDependency where TEntity : class, IEntity<string>
    {
    }
}
