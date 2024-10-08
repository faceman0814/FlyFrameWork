﻿using FlyFramework.Dependencys;

using System.Threading;
using System.Threading.Tasks;
namespace FlyFramework.Uow
{
    public interface IUnitOfWork : IScopedDependency
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
        Task BeginAsync(CancellationToken cancellationToken = default);
    }
}
