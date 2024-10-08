using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static T As<T>(this object obj) where T : class
        {
            return (T)obj;
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


        public static bool IsIn<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }

        public static bool HasValue(this string val)
        {
            return !string.IsNullOrWhiteSpace(val);
        }
    }
}
