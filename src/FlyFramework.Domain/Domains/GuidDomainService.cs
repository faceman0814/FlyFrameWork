using FlyFramework.Common.Dependencys;
using FlyFramework.Common.Extentions.Object;
using FlyFramework.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Domain.Domains
{
    public abstract class GuidDomainService<TEntity> : DomainService<TEntity, string>, IGuidDomainService<TEntity>, ITransientDependency where TEntity : class, IFullAuditedEntity<string>
    {
        public GuidDomainService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public new async Task<TEntity> Create(TEntity entity)
        {
            await ValidateOnCreateOrUpdate(entity);
            entity.Id = Guid.NewGuid().ToString("N");
            entity.CreationTime = DateTime.Now;
            await Repo.InsertAsync(entity);
            return entity;
        }

        public new async Task<TEntity> Update(TEntity entity)
        {
            await ValidateOnCreateOrUpdate(entity);
            TEntity oldEntity = entity.JsonClone();
            oldEntity.LastModificationTime = DateTime.Now;
            await Repo.UpdateAsync(oldEntity);
            return entity;
        }

        public new async Task Delete(TEntity entity)
        {
            await ValidateOnDelete(entity);
            ClearNavigationProp(entity);
            entity.IsDeleted = true;
            entity.DeletionTime = DateTime.Now;
            await Repo.UpdateAsync(entity);
        }
    }
}
