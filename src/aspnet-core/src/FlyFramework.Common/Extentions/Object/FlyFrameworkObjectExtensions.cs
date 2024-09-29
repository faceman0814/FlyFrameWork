using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FlyFramework.Extentions.Object
{
    /// <summary>
    /// FlyFramework对象扩展
    /// </summary>
    public static class FlyFrameworkObjectExtensions
    {
        public static IObjectExtensions Instance = new ObjectExtensions();

        /// <summary>
        /// Json深拷贝
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">实体</param>
        /// <returns></returns>
        public static T JsonClone<T>(this T obj)
        {
            return Instance.JsonClone(obj);
        }

        /// <summary>
        /// Json深拷贝，忽略null值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T JsonCloneNone<T>(this T obj)
        {
            return Instance.JsonCloneNone(obj);
        }

        /// <summary>
        /// Json序列化，驼峰式命名
        /// </summary>
        /// <param name="jsonObj"></param>
        /// <returns></returns>
        public static string SerializeJsonCamelCase(this object jsonObj)
        {
            return Instance.SerializeJsonCamelCase(jsonObj);
        }

        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="jsonObj"></param>
        /// <returns></returns>
        public static string SerializeJson(this object jsonObj)
        {
            return Instance.SerializeJson(jsonObj);
        }

        /// <summary>
        /// Json反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string jsonContent)
        {
            return Instance.Deserialize<T>(jsonContent);
        }

        public static T To<T>(this object obj) where T : struct
        {
            if (typeof(T) == typeof(Guid) || typeof(T) == typeof(TimeSpan))
            {
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString());
            }

            if (typeof(T).IsEnum)
            {
                if (Enum.IsDefined(typeof(T), obj))
                {
                    return (T)Enum.Parse(typeof(T), obj.ToString());
                }

                throw new ArgumentException($"Enum type undefined '{obj}'.");
            }

            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T As<T>(this object obj)
       where T : class
        {
            return (T)obj;
        }

        private static readonly ConcurrentDictionary<string, PropertyInfo?> _cachedObjectProperties = new();

        public static void TrySetProperty<TObject, TValue>(
            TObject obj,
            Expression<Func<TObject, TValue>> propertySelector,
            Func<TValue> valueFactory,
            params Type[] ignoreAttributeTypes)
        {
            TrySetProperty(obj, propertySelector, x => valueFactory(), ignoreAttributeTypes);
        }

        public static void TrySetProperty<TObject, TValue>(
            TObject obj,
            Expression<Func<TObject, TValue>> propertySelector,
            Func<TObject, TValue> valueFactory,
            params Type[]? ignoreAttributeTypes)
        {
            var cacheKey =
                $"{obj?.GetType().FullName}-{propertySelector}-{(ignoreAttributeTypes != null ? "-" + string.Join("-", ignoreAttributeTypes.Select(x => x.FullName)) : "")}";

            var property = _cachedObjectProperties.GetOrAdd(cacheKey, PropertyFactory);

            property?.SetValue(obj, valueFactory(obj));
            return;

            PropertyInfo? PropertyFactory(string _)
            {
                MemberExpression? memberExpression;
                switch (propertySelector.Body.NodeType)
                {
                    case ExpressionType.Convert:
                        {
                            memberExpression = propertySelector.Body.As<UnaryExpression>().Operand as MemberExpression;
                            break;
                        }
                    case ExpressionType.MemberAccess:
                        {
                            memberExpression = propertySelector.Body.As<MemberExpression>();
                            break;
                        }
                    default:
                        {
                            return null;
                        }
                }

                if (memberExpression == null)
                {
                    return null;
                }

                var propertyInfo = obj?.GetType()
                    .GetProperties()
                    .FirstOrDefault(x => x.Name == memberExpression.Member.Name);

                if (propertyInfo == null)
                {
                    return null;
                }

                var propPrivateSetMethod = propertyInfo.GetSetMethod(true);
                if (propPrivateSetMethod == null)
                {
                    return null;
                }

                if (ignoreAttributeTypes != null && ignoreAttributeTypes.Any(ignoreAttribute => propertyInfo.IsDefined(ignoreAttribute, true)))
                {
                    return null;
                }

                return propertyInfo;
            }
        }
    }
}
