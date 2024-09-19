using FlyFramework.Common.ErrorExceptions;
using FlyFramework.Repositories.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FlyFramework.Repositories.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        protected DbContext _context { get; }
        private IDbContextTransaction _transaction;

        public UnitOfWork(IDbContextProvider dbContextProvider)
        {
            _context = dbContextProvider.GetDbContext();
        }
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                await RollbackTransactionAsync();
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

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
