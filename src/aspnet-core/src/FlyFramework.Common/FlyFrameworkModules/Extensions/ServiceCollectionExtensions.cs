using FlyFramework.Dependencys;
using FlyFramework.Extentions;
using FlyFramework.FlyFrameworkModules.Interface;
using FlyFramework.FlyFrameworkModules.Modules;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace FlyFramework.FlyFrameworkModules.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加一个对象访问器，如果该对象已经注册过，则抛出异常。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>对象访问器</returns>
        public static ObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services)
        {
            // 检查是否已经注册过该对象
            if (services.Any(s => s.ServiceType == typeof(IObjectAccessor<T>)))
            {
                throw new Exception("该对象已经注册成功过 " + typeof(T).AssemblyQualifiedName);
            }
            // 创建一个新的对象访问器
            var accessor = new ObjectAccessor<T>();
            // 将对象访问器添加到服务集合的开头，以便快速检索
            services.Insert(0, ServiceDescriptor.Singleton<IObjectAccessor<T>>(accessor));

            // 再次检查是否已经注册过该对象
            if (services.Any(s => s.ServiceType == typeof(ObjectAccessor<T>)))
            {
                throw new Exception("该对象已经注册成功过: " + typeof(T).AssemblyQualifiedName);
            }

            // 将对象访问器添加到服务集合的开头
            services.Insert(0, ServiceDescriptor.Singleton(accessor));

            return accessor;
        }

        /// <summary>
        /// 添加一个带有指定对象的对象访问器，如果该对象已经注册过，则抛出异常。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="obj">要注册的对象</param>
        /// <returns>对象访问器</returns>
        public static ObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services, T obj)
        {
            // 检查是否已经注册过该对象
            if (services.Any(s => s.ServiceType == typeof(IObjectAccessor<T>)))
            {
                throw new Exception("该对象已经注册成功过 " + typeof(T).AssemblyQualifiedName);
            }
            // 创建一个新的对象访问器，并设置其对象
            var accessor = new ObjectAccessor<T>(obj);
            // 将对象访问器添加到服务集合的开头，以便快速检索
            services.Insert(0, ServiceDescriptor.Singleton<IObjectAccessor<T>>(accessor));

            // 再次检查是否已经注册过该对象
            if (services.Any(s => s.ServiceType == typeof(ObjectAccessor<T>)))
            {
                throw new Exception("该对象已经注册成功过: " + typeof(T).AssemblyQualifiedName);
            }

            // 将对象访问器添加到服务集合的开头
            services.Insert(0, ServiceDescriptor.Singleton(accessor));

            return accessor;
        }

        /// <summary>
        /// 检查服务集合是否为 null，如果为 null 则抛出异常。
        /// </summary>
        /// <param name="services">服务集合</param>
        public static void CheckNull(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentException("services is null");
            }
        }

        /// <summary>
        /// 获取单例实例，如果找不到则返回 null。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>单例实例或 null</returns>
        public static T GetSingletonInstanceOrNull<T>(this IServiceCollection services) where T : class
        {
            // 查找服务类型为 T 的服务描述符
            var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(T));
            // 返回服务描述符的实现实例，并进行类型转换
            return serviceDescriptor?.ImplementationInstance as T;
        }

        /// <summary>
        /// 检查服务集合是否为 null，如果为 null 则抛出异常。
        /// </summary>
        /// <param name="services">服务集合</param>
        public static void ChcekNull(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException("IServiceCollection为空");
            }
        }

        /// <summary>
        /// 获取单例实例，如果找不到则抛出异常。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>单例实例</returns>
        public static T GetSingletonInstance<T>(this IServiceCollection services) where T : class
        {
            // 获取单例实例或 null
            var service = services.GetSingletonInstanceOrNull<T>();
            // 如果服务为 null，则抛出异常
            if (service == null)
            {
                throw new InvalidOperationException("Could not find singleton service: " + typeof(T).AssemblyQualifiedName);
            }

            return service;
        }

        /// <summary>
        /// 添加指定类型的程序集中的服务。
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddAssemblyOf<T>(this IServiceCollection services)
        {
            // 添加指定类型的程序集
            return services.AddAssembly(typeof(T).GetTypeInfo().Assembly);
        }

        /// <summary>
        /// 添加指定程序集中的服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="assembly">程序集</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddAssembly(this IServiceCollection services, Assembly assembly)
        {
            // 使用常规注册器添加程序集中的服务
            new ConventionalRegistrar().AddAssembly(services, assembly);

            return services;
        }

        /// <summary>
        /// Checks whatever given collection object is null or has no item.
        /// </summary>
        [ContractAnnotation("source:null => true")]
        public static bool IsNullOrEmpty<T>(this ICollection<T>? source)
        {
            return source == null || source.Count <= 0;
        }

        /// <summary>
        /// Adds an item to the collection if it's not already in the collection.
        /// </summary>
        /// <param name="source">The collection</param>
        /// <param name="item">Item to check and add</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns True if added, returns False if not.</returns>
        public static bool AddIfNotContains<T>([NotNull] this ICollection<T> source, T item)
        {
            Check.NotNull(source, nameof(source));

            if (source.Contains(item))
            {
                return false;
            }

            source.Add(item);
            return true;
        }

        /// <summary>
        /// Adds items to the collection which are not already in the collection.
        /// </summary>
        /// <param name="source">The collection</param>
        /// <param name="items">Item to check and add</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns the added items.</returns>
        public static IEnumerable<T> AddIfNotContains<T>([NotNull] this ICollection<T> source, IEnumerable<T> items)
        {
            Check.NotNull(source, nameof(source));

            var addedItems = new List<T>();

            foreach (var item in items)
            {
                if (source.Contains(item))
                {
                    continue;
                }

                source.Add(item);
                addedItems.Add(item);
            }

            return addedItems;
        }

        /// <summary>
        /// Adds an item to the collection if it's not already in the collection based on the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="source">The collection</param>
        /// <param name="predicate">The condition to decide if the item is already in the collection</param>
        /// <param name="itemFactory">A factory that returns the item</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns True if added, returns False if not.</returns>
        public static bool AddIfNotContains<T>([NotNull] this ICollection<T> source, [NotNull] Func<T, bool> predicate, [NotNull] Func<T> itemFactory)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));
            Check.NotNull(itemFactory, nameof(itemFactory));

            if (source.Any(predicate))
            {
                return false;
            }

            source.Add(itemFactory());
            return true;
        }

        /// <summary>
        /// Removes all items from the collection those satisfy the given <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <param name="source">The collection</param>
        /// <param name="predicate">The condition to remove the items</param>
        /// <returns>List of removed items</returns>
        public static IList<T> RemoveAll<T>([NotNull] this ICollection<T> source, Func<T, bool> predicate)
        {
            var items = source.Where(predicate).ToList();

            foreach (var item in items)
            {
                source.Remove(item);
            }

            return items;
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <param name="source">The collection</param>
        /// <param name="items">Items to be removed from the list</param>
        public static void RemoveAll<T>([NotNull] this ICollection<T> source, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                source.Remove(item);
            }
        }
    }
}