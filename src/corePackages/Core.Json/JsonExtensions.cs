using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Core.Json
{
    public static class JsonExtensions
    {
        private static readonly CustomCamelCasePropertyNamesContractResolver _sharedCamelCasePropertyNamesContractResolver = new CustomCamelCasePropertyNamesContractResolver();
        private static readonly CustomContractResolver _sharedContractResolver = new CustomContractResolver();

        public static string? ToJsonString(this object obj, bool camelCase = false, bool indented = false)
        {
            var settings = new JsonSerializerSettings();

            if (camelCase)
            {
                settings.ContractResolver = _sharedCamelCasePropertyNamesContractResolver;
            }
            else
            {
                settings.ContractResolver = _sharedContractResolver;
            }

            if (indented)
            {
                settings.Formatting = Formatting.Indented;
            }

            return ToJsonString(obj, settings);
        }

        public static string? ToJsonString(this object obj, JsonSerializerSettings settings)
        {
            return obj != null
                ? JsonConvert.SerializeObject(obj, settings)
                : default(string);
        }

        public static T? FromJsonString<T>(this string value)
        {
            return value.FromJsonString<T>(new JsonSerializerSettings());
        }

        public static T? FromJsonString<T>(this string value, JsonSerializerSettings settings)
        {
            return value != null
                ? JsonConvert.DeserializeObject<T>(value, settings)
                : default(T);
        }

        public static object? FromJsonString(this string value, [NotNull] Type type, JsonSerializerSettings settings)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return value != null
                ? JsonConvert.DeserializeObject(value, type, settings)
                : null;
        }
    }
}