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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace FlyFramework
{
    public class FlyFrameworkDbContext : DbContextBase
    {
        public bool IgnoreDeleteFilter { get; set; } = false;

        public bool SuppressAutoSetTenantId { get; set; }

        public FlyFrameworkDbContext(DbContextOptions<FlyFrameworkDbContext> options)
        : base(options)
        {
        }

        /// <summary>
        /// 动态注册实体和筛选器
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //获取FlyFrameworkCoreModule的程序集
            var coreModule = typeof(FlyFrameworkCoreModule).Assembly;

            //获取所有继承Entity<string>的实体类型
            List<Type> entityTypes = coreModule.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Entity<string>)) && !t.IsAbstract)
                .ToList();

            MethodInfo? configureFilters = typeof(FlyFrameworkDbContext).GetMethod(
                nameof(ConfigureFilters),
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            if (configureFilters == null) throw new ArgumentNullException(nameof(configureFilters));

            foreach (Type entityType in entityTypes)
            {
                // 注册实体
                modelBuilder.Entity(entityType);

                // 注册筛选器
                configureFilters
                    .MakeGenericMethod(entityType)
                    .Invoke(this, new object[] { modelBuilder, entityType });
            }
        }

        protected virtual void ConfigureFilters<TEntity>(ModelBuilder builder, Type entityType)
           where TEntity : class
        {
            Expression<Func<TEntity, bool>>? expression = null;

            if (typeof(ISoftDelete).IsAssignableFrom(entityType))
            {
                expression = e => IgnoreDeleteFilter || !EF.Property<bool>(e, "IsDeleted");
            }

            if (typeof(IMayHaveTenant).IsAssignableFrom(entityType))
            {
                Expression<Func<TEntity, bool>> tenantExpression = e => EF.Property<int>(e, "TenantId") == 1;
                expression = expression == null ? tenantExpression : CombineExpressions(expression, tenantExpression);
            }

            if (expression == null) return;

            builder.Entity<TEntity>().HasQueryFilter(expression);
        }

        protected virtual Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expression1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expression2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }

        class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression? node)
            {
                if (node == _oldValue)
                {
                    return _newValue;
                }

                return base.Visit(node)!;
            }
        }
        public DbSet<UserRole> UserRole { get; set; }
    }
}
