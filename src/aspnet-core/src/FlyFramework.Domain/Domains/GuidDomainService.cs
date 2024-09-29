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
        private IUserSession _userSession { get; set; }
        public GuidDomainService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _userSession = serviceProvider.GetService<IUserSession>();
        }
        public new async Task<TEntity> Create(TEntity entity)
        {
            await ValidateOnCreateOrUpdate(entity);
            entity.Id = Guid.NewGuid().ToString("N");
            entity.CreationTime = DateTime.Now;
            entity.CreatorUserId = _userSession.UserId;
            entity.CreatorUserName = _userSession.UserName;
            await Repo.InsertAsync(entity);
            return entity;
        }

        public new async Task<TEntity> Update(TEntity entity)
        {
            await ValidateOnCreateOrUpdate(entity);
            TEntity oldEntity = entity.JsonClone();
            oldEntity.LastModificationTime = DateTime.Now;
            oldEntity.LastModifierUserId = _userSession.UserId;
            oldEntity.LastModifierUserName = _userSession.UserName;
            await Repo.UpdateAsync(oldEntity);
            return entity;
        }

        public new async Task Delete(TEntity entity)
        {
            await ValidateOnDelete(entity);
            ClearNavigationProp(entity);
            entity.IsDeleted = true;
            entity.DeletionTime = DateTime.Now;
            entity.DeleterUserId = _userSession.UserId;
            entity.DeleterUserName = _userSession.UserName;
            await Repo.UpdateAsync(entity);
        }
    }
}
