using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Extentions.ContractResolvers
{
    public class NonPublicSetterContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);
            if (!jsonProperty.Writable)
            {
                PropertyInfo propertyInfo = member as PropertyInfo;
                if (propertyInfo != null)
                {
                    bool writable = propertyInfo.GetSetMethod(nonPublic: true) != null;
                    jsonProperty.Writable = writable;
                }
            }

            return jsonProperty;
        }
    }
}
