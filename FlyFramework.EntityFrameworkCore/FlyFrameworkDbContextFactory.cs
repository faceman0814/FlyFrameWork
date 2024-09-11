// Licensed to the .NET YoyoBoot under one or more agreements.
// The .NET YoyoBoot licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FlyFramework.EntityFrameworkCore
{
    public class FlyFrameworkDbContextFactory : IDesignTimeDbContextFactory<FlyFrameworkDbContext>
    {
        private static readonly IServiceProvider _serviceProvider;

        public FlyFrameworkDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FlyFrameworkDbContext>();
            optionsBuilder.UseSqlServer("Server=data.dev.52abp.com,1434;Database=FlyDev;User ID=sa;Password=bb123456??;MultipleActiveResultSets=True;TrustServerCertificate=True;");
            return new FlyFrameworkDbContext(optionsBuilder.Options);
        }
    }
}
