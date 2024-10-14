using FlyFramework.Dependencys;
using FlyFramework.Entities;
using FlyFramework.Extentions.Object;
using FlyFramework.UserSessions;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Threading.Tasks;

namespace FlyFramework.Domains
{
    public abstract class GuidDomainService<TEntity> : DomainService<TEntity, string>, IGuidDomainService<TEntity>, ITransientDependency where TEntity : class, IFullAuditedEntity<string>
    {
        public GuidDomainService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public new async Task<TEntity> Create(TEntity entity)
        {
            await ValidateOnCreateOrUpdate(entity);
            await Repo.InsertAsync(entity);
            return entity;
        }

        public new async Task<TEntity> Update(TEntity entity)
        {
            await ValidateOnCreateOrUpdate(entity);
            TEntity oldEntity = entity.JsonClone();
            await Repo.UpdateAsync(oldEntity);
            return entity;
        }

        public new async Task Delete(TEntity entity)
        {
            await ValidateOnDelete(entity);
            ClearNavigationProp(entity);
            await Repo.UpdateAsync(entity);
        }
    }
}
