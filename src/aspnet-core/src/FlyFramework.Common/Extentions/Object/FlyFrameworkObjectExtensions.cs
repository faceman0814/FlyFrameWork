using System;
using System.Collections.Generic;
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
    }
}
