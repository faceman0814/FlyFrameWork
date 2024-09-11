using FlyFramework.Core.TestService;

using Microsoft.EntityFrameworkCore;

namespace FlyFramework.EntityFrameworkCore
{
    public class FlyFrameworkDbContext : DbContext
    {
        public FlyFrameworkDbContext(DbContextOptions<FlyFrameworkDbContext> options)
        : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //从当前程序集中加载实现了IDesignTimeDbContextFactory接口的配置类
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            //等价
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Book> Book { get; set; }
        public DbSet<Category> Category { get; set; }
    }
}
