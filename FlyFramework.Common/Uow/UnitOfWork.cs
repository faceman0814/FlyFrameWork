using FlyFramework.Common.Repositories;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        protected DbContext _context { get; }

        public UnitOfWork(IDbContextProvider dbContextProvider)
        {
            _context = dbContextProvider.GetDbContext();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
