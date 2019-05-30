using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
namespace Detrack.ElasticRoute.Tools
{
    public class StopContractResolver : DefaultContractResolver
    {
        public StopContractResolver()
        {
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);

            if (!jsonProperty.Writable)
            {
                var property = member as PropertyInfo;
                if(property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true);
                    jsonProperty.Writable = true;
                }
            }

            return jsonProperty;
        }
    }
}
