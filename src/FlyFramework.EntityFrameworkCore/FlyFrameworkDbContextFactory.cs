// Licensed to the .NET YoyoBoot under one or more agreements.
// The .NET YoyoBoot licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using System.IO;

namespace FlyFramework.EntityFrameworkCore
{
    public class FlyFrameworkDbContextFactory : IDesignTimeDbContextFactory<FlyFrameworkDbContext>
    {
        public FlyFrameworkDbContext CreateDbContext(string[] args)
        {
            // 设定配置文件路径
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(Directory.GetCurrentDirectory()));
            var path = Path.Combine(directoryInfo.FullName, "FlyFramework.WebHost");
            Console.WriteLine("工作目录：{0}", path);
            // 配置 builder 来读取 appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json")
                .Build();
            // 从配置中读取连接字符串
            var connectionString = configuration.GetConnectionString("Default");
            Console.WriteLine("迁移使用数据库连接字符串：{0}", connectionString);
            var optionsBuilder = new DbContextOptionsBuilder<FlyFrameworkDbContext>();
            //获取appsettings配置
            optionsBuilder.UseSqlServer(connectionString);
            return new FlyFrameworkDbContext(optionsBuilder.Options);
        }
    }
}
