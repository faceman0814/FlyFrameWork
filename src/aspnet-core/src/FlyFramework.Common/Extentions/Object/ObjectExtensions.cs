using FlyFramework.Extentions.ContractResolvers;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FlyFramework.Extentions.Object
{
    /// <summary>
    /// 对象扩展类
    /// </summary>
    public class ObjectExtensions : IObjectExtensions
    {
        /// <summary>
        /// 默认json序列化设置
        /// </summary>
        protected virtual JsonSerializerSettings DefaultJsonSerializerSettings { get; set; } = new JsonSerializerSettings
        {
            ContractResolver = new NonPublicSetterContractResolver(),
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };

        /// <summary>
        /// 驼峰命名JSON序列化配置
        /// </summary>
        protected virtual JsonSerializerSettings CamelCaseJsonSerializerSettings { get; set; } = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };


        public virtual T Deserialize<T>(string jsonContent)
        {
            return JsonConvert.DeserializeObject<T>(jsonContent, DefaultJsonSerializerSettings);
        }



        public virtual T JsonClone<T>(T obj)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj, DefaultJsonSerializerSettings), DefaultJsonSerializerSettings);
        }


        public virtual T JsonCloneNone<T>(T obj)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
        }


        public virtual string SerializeJsonCamelCase(object jsonObj)
        {
            if (jsonObj == null)
            {
                return string.Empty;
            }

            return JsonConvert.SerializeObject(jsonObj, CamelCaseJsonSerializerSettings);
        }

        public virtual string SerializeJson(object jsonObj)
        {
            if (jsonObj == null)
            {
                return string.Empty;
            }

            return JsonConvert.SerializeObject(jsonObj, DefaultJsonSerializerSettings);
        }

        /// <summary>
        /// 实体类转换，要求两个类中的成员一致
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="pModel"></param>
        /// <returns></returns>
        public static T ConvertModel<P, T>(in P pModel)
        {
            T ret = System.Activator.CreateInstance<T>();

            List<PropertyInfo> p_pis = pModel.GetType().GetProperties().ToList();
            PropertyInfo[] t_pis = typeof(T).GetProperties();

            foreach (PropertyInfo pi in t_pis)
            {
                //可写入数据
                if (pi.CanWrite)
                {
                    //忽略大小写
                    var name = p_pis.Find(s => s.Name.ToLower() == pi.Name.ToLower());
                    if (name != null && pi.PropertyType.Name == name.PropertyType.Name)
                    {
                        pi.SetValue(ret, name.GetValue(pModel, null), null);
                    }
                }
            }

            return ret;
        }
    }
}
