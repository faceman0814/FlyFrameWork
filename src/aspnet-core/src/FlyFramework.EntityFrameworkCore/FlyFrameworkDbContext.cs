using FlyFramework.Entities;
using FlyFramework.Extentions;
using FlyFramework.Extentions.Object;
using FlyFramework.UserModule;
using FlyFramework.UserSessions;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

using System;
using System.Linq;
namespace FlyFramework
{
    public class FlyFrameworkDbContext : DbContextBase
    {

        public bool SuppressAutoSetTenantId { get; set; }

        public FlyFrameworkDbContext(DbContextOptions<FlyFrameworkDbContext> options)
        : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> User { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<Role> Role { get; set; }
    }
}
