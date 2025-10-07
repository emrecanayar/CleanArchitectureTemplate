using Newtonsoft.Json;

namespace Core.Json
{
    /// <summary>
    /// Defines helper methods to work with JSON.
    /// </summary>
    public static class JsonSerializationHelper
    {
        private const char _typeSeperator = '|';

        /// <summary>
        /// Serializes an object with a type information included.
        /// So, it can be deserialized using <see cref="DeserializeWithType"/> method later.
        /// </summary>
        /// <returns>string.</returns>
        public static string SerializeWithType(object obj)
        {
            return SerializeWithType(obj, obj.GetType());
        }

        /// <summary>
        /// Serializes an object with a type information included.
        /// So, it can be deserialized using <see cref="DeserializeWithType"/> method later.
        /// </summary>
        /// <returns>string.</returns>
        public static string SerializeWithType(object obj, Type type)
        {
            var serialized = obj.ToJsonString();

            return string.Format(
                "{0}{1}{2}",
                type.AssemblyQualifiedName,
                _typeSeperator,
                serialized);
        }

        /// <summary>
        /// Deserializes an object serialized with <see cref="SerializeWithType(object)"/> methods.
        /// </summary>
        /// <returns>T.</returns>
        public static T? DeserializeWithType<T>(string serializedObj)
        {
            return (T?)DeserializeWithType(serializedObj);
        }

        /// <summary>
        /// Deserializes an object serialized with <see cref="SerializeWithType(object)"/> methods.
        /// </summary>
        /// <returns>.</returns>
        public static object? DeserializeWithType(string serializedObj)
        {
            var typeSeperatorIndex = serializedObj.IndexOf(_typeSeperator);
            var type = Type.GetType(serializedObj.Substring(0, typeSeperatorIndex));
            var serialized = serializedObj.Substring(typeSeperatorIndex + 1);

            var options = new JsonSerializerSettings
            {
                ContractResolver = new CustomCamelCasePropertyNamesContractResolver(),
            };

            return JsonConvert.DeserializeObject(serialized, type, options);
        }
    }
}
