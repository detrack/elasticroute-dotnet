using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
namespace Detrack.ElasticRoute.Tools
{
    public class CustomContractResolver : DefaultContractResolver
    {
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

            jsonProperty.ShouldSerialize = (instance) =>
            {
                if(instance is BaseModel)
                {
                    BaseModel o = instance as BaseModel;
                    return o.ModifiedProperties.Contains(jsonProperty.UnderlyingName);
                }
                else
                {
                    return ! jsonProperty.Ignored;
                }
            };

            return jsonProperty;
        }
    }
}
