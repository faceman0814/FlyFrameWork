using FlyFramework.Repositories;

using Microsoft.EntityFrameworkCore;

namespace FlyFramework
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
