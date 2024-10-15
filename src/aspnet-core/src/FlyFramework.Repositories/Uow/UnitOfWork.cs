using FlyFramework.ErrorExceptions;
using FlyFramework.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlyFramework.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        protected DbContext _context { get; }
        private IDbContextTransaction _transaction;
        public UnitOfWork(IDbContextProvider dbContextProvider)
        {
            _context = dbContextProvider.GetDbContext();
        }
        public async Task BeginAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                if (_transaction != null)
                {
                    await _transaction.CommitAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                await RollbackAsync(cancellationToken);
                throw new UserFriendlyException(ex.InnerException?.Message ?? ex.Message);
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public DbContext GetDbContext()
        {
            //获取DbContext
            return _context;

        }

        public Task<DbContext> GetDbContextAsync(CancellationToken cancellationToken = default)
        {
            //获取DbContext
            return Task.FromResult(_context);
        }
    }
}
