using FlyFramework.Common.Extentions.ContractResolvers;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FlyFramework.Common.Extentions
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
    }
}
