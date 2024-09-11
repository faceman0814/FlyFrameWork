using FlyFramework.Common.Dependencys;
using FlyFramework.Common.DomainBase;
using FlyFramework.Common.Entities;
using FlyFramework.Common.Repositories;

namespace FlyFramework.Common.Domain
{
    public interface IDomainService<TEntity, TPrimaryKey> : IDomainBaseService, ITransientDependency where TEntity : class, IEntity<TPrimaryKey>
    {
        //
        // 摘要:
        //     TEntity 仓储
        IRepository<TEntity, TPrimaryKey> Repo { get; }

        //
        // 摘要:
        //     TEntity 查询器
        IQueryable<TEntity> Query { get; }

        //
        // 摘要:
        //     TEntity 查询器 - 不追踪
        IQueryable<TEntity> QueryAsNoTracking { get; }

        //
        // 摘要:
        //     获取 TEntity 并包含导航属性的查询器
        IQueryable<TEntity> GetNdoIncludeQuery();

        //
        // 摘要:
        //     根据id查找 并包含导航属性
        //
        // 参数:
        //   id:
        Task<TEntity> FindNdoByIdWithInclude(TPrimaryKey id);

        //
        // 摘要:
        //     获取一个 对象的新 Id
        TPrimaryKey NewNdoId();

        //
        // 摘要:
        //     根据id查找
        //
        // 参数:
        //   id:
        //     Id
        Task<TEntity> FindNdoById(TPrimaryKey id);

        //
        // 摘要:
        //     检查 是否存在
        //
        // 参数:
        //   id:
        Task<TEntity> IsExistNdo(TPrimaryKey id);

        //
        // 摘要:
        //     直接将 对象新增到数据库，经过校验方法
        //
        // 参数:
        //   ndo:
        //     实例
        Task<TEntity> CreateNdo(TEntity ndo);

        //
        // 摘要:
        //     直接将 对象更新到数据库，不经过校验方法
        //
        // 参数:
        //   ndo:
        Task<TEntity> UpdateNdo(TEntity ndo);

        //
        // 摘要:
        //     根据id删除
        //
        // 参数:
        //   id:
        Task DeleteNdo(TPrimaryKey id);

        //
        // 摘要:
        //     根据对象删除
        //
        // 参数:
        //   ndo:
        Task DeleteNdo(TEntity ndo);

        //
        // 摘要:
        //     判断Ndo是否可以删除
        //
        // 参数:
        //   ndo:
        //     ndo对象
        Task ValidateNdoOnDelete(TEntity ndo);

        //
        // 摘要:
        //     校验Ndo数据正确性 - 新增和修改
        //
        // 参数:
        //   ndo:
        //     TNdo 实例
        //
        //   NdoBase:
        //     TNdoBase 实例
        Task ValidateNdoOnCreateOrUpdate(TEntity ndo);
    }
}
