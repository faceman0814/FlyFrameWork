using FlyFramework.UserModule;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using System.Threading;
using System.Threading.Tasks;

namespace FlyFramework
{
    public abstract class DbContextBase : IdentityDbContext<User, Role, string>
    {
        protected DbContextBase()
        {
        }

        protected DbContextBase(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        protected virtual void BeforeSaveChanges()
        {

        }

        protected virtual void AfterSaveChanges()
        {

        }

        public override int SaveChanges()
        {
            BeforeSaveChanges();
            var result = base.SaveChanges();
            AfterSaveChanges();
            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            BeforeSaveChanges();
            var result = await base.SaveChangesAsync(cancellationToken);
            AfterSaveChanges();
            return result;
        }
    }
}
