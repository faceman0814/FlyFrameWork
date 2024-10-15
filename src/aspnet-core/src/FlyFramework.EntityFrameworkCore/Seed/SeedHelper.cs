using FlyFramework.Seed.Host;
using FlyFramework.Uow;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using ServiceStack.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FlyFramework.Seed
{
    public class SeedHelper
    {
        public static void SeedHostDb(FlyFrameworkDbContext context, IServiceProvider serviceProvider)
        {
            context.SuppressAutoSetTenantId = true;
            // 初始化宿主的种子数据
            new InitialHostDbBuilder(context, serviceProvider).Create();
        }

        public static void SeedHostDb(IServiceProvider serviceProvider)
        {
            WithDbContext<FlyFrameworkDbContext>(serviceProvider, SeedHostDb);
        }


        private static void WithDbContext<TDbContext>(IServiceProvider serviceProvider, Action<TDbContext, IServiceProvider> contextAction)
            where TDbContext : DbContext
        {
            // 创建一个事务
            using (var uowManager = serviceProvider.GetService<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Begin())
                {
                    var context = uowManager.Current.GetDbContext();

                    contextAction((TDbContext)context, serviceProvider);

                    uow.SaveChangesAsync();
                }
            }
        }
    }
}
