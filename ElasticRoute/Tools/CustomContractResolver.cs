using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
namespace Detrack.ElasticRoute.Tools
{
    /// <summary>
    /// Custom contract resolver intended for use by classes that extend <see cref="BaseModel"/>. Attempts to write fields with an internal setter, and only serialise fields that have been modified.
    /// </summary>
    public class CustomContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Creates the property.  Attempts to write fields with an internal setter, and only serialise fields that have been modified.
        /// </summary>
        /// <returns>The property.</returns>
        /// <param name="member">Member.</param>
        /// <param name="memberSerialization">Member serialization.</param>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);

            if (!jsonProperty.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true);
                    jsonProperty.Writable = true;
                }
            }

            jsonProperty.ShouldSerialize = (instance) =>
            {
                if (instance is BaseModel)
                {
                    BaseModel o = instance as BaseModel;
                    return o.ModifiedProperties.Contains(jsonProperty.UnderlyingName);
                }
                else
                {
                    return !jsonProperty.Ignored;
                }
            };

            return jsonProperty;
        }
    }
}
