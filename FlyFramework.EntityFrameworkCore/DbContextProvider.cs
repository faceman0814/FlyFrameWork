using FlyFramework.Common.Repositories;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.EntityFrameworkCore
{
    public class DbContextProvider : IDbContextProvider
    {
        private readonly FlyFrameworkDbContext _context;

        public DbContextProvider(FlyFrameworkDbContext context)
        {
            _context = context;
        }

        public DbContext GetDbContext()
        {
            return _context;
        }
    }
}
