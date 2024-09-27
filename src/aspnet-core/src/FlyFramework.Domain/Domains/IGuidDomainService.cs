using FlyFramework.Common.Dependencys;
using FlyFramework.Domain.Entities;
namespace FlyFramework.Domain.Domains
{
    public interface IGuidDomainService<TEntity> : IDomainService<TEntity, string>, ITransientDependency where TEntity : class, IFullAuditedEntity<string>
    {
    }
}
