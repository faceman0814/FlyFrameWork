using FlyFramework.Common.Dependencys;
using FlyFramework.Common.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Domain
{
    public interface IGuidDomainService<TEntity> : IDomainService<TEntity, string>, ITransientDependency where TEntity : class, IFullAuditedEntity<string>
    {
    }
}
