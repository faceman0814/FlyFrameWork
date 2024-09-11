using FlyFramework.Common.Dependencys;
using FlyFramework.Common.Entities;
using FlyFramework.Common.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Domain
{
    public abstract class DomainService<TEntity, TPrimaryKey> : IDomainService<TEntity, TPrimaryKey>, ITransientDependency where TEntity : class, IEntity<TPrimaryKey>
    {
        public virtual IServiceProvider ServiceProvider { get; private set; }
        public IRepository<TEntity, TPrimaryKey> Repo { get; }
        public IQueryable<TEntity> Query => Repo.GetAll();
        public IQueryable<TEntity> QueryAsNoTracking => Query.AsNoTracking();
        public DomainService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Repo = serviceProvider.GetRequiredService<IRepository<TEntity, TPrimaryKey>>();
        }

        public abstract IQueryable<TEntity> GetIncludeQuery();

        //public async  TPrimaryKey NewId();

        public abstract Task ValidateOnDelete(TEntity entity);

        public abstract Task ValidateOnCreateOrUpdate(TEntity entity);

        public async Task<TEntity> FindByIdWithInclude(TPrimaryKey id)
        {
            return await GetIncludeQuery().FirstOrDefaultAsync((TEntity x) => x.Id.Equals(id));
        }

        public async Task<TEntity> FindById(TPrimaryKey id)
        {
            return await QueryAsNoTracking.FirstOrDefaultAsync((TEntity x) => x.Id.Equals(id));
        }

        public async Task<TEntity> IsExist(TPrimaryKey id)
        {
            TEntity val = await FindById(id);
            if (val == null)
            {
                //throw new UserFriendlyException(L("Error"), L("NullError", Clock.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }

            return val;
        }

        public async Task<TEntity> Create(TEntity entity)
        {
            await ValidateOnCreateOrUpdate(entity);
            await Repo.InsertAsync(entity);
            return entity;
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            await ValidateOnCreateOrUpdate(entity);
            //TEntity oldEntity = entity.JsonClone();
            //await Repo.UpdateAsync(oldEntity);
            return entity;
        }

        public async Task Delete(TPrimaryKey id)
        {
            await Delete(await FindByIdWithInclude(id));
        }

        public async Task Delete(TEntity entity)
        {
            await ValidateOnDelete(entity);
            //ClearNavigationProp(entity);
            await Repo.DeleteAsync(entity);
        }

    }
}
