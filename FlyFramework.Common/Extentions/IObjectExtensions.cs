using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Extentions
{
    public interface IObjectExtensions
    {
        /// <summary>
        /// JSON字符串反序列化为指定类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        T Deserialize<T>(string jsonContent);
        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        T JsonClone<T>(T obj);
        /// <summary>
        /// 深拷贝，忽略null值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        T JsonCloneNone<T>(T obj);
        /// <summary>
        /// JSON字符串序列化，驼峰命名
        /// </summary>
        /// <param name="jsonObj"></param>
        /// <returns></returns>
        string SerializeJsonCamelCase(object jsonObj);
        /// <summary>
        /// JSON字符串序列化
        /// </summary>
        /// <param name="jsonObj"></param>
        /// <returns></returns>
        string SerializeJson(object jsonObj);
    }
}
