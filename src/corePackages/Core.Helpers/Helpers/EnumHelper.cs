using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Core.Helpers.Helpers
{
    public static class EnumHelper<T>
        where T : struct, Enum
    {
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }

            return enumValues;
        }

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }

        public static string GetDisplayValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString()) ?? throw new InvalidOperationException($"Unable to find the field '{value}' on type '{value.GetType().Name}'.");

            DisplayAttribute[]? descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null || descriptionAttributes.Length == 0)
            {
                return value.ToString();
            }

            var displayAttribute = descriptionAttributes[0];

            if (displayAttribute.ResourceType != null && displayAttribute.Name != null)
            {
                return LookupResource(displayAttribute.ResourceType, displayAttribute.Name);
            }

            return displayAttribute.Name ?? value.ToString();
        }

        public static Dictionary<int, string> GetEnumValuesAndNames(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("enumType must be an Enum type");
            }

            Dictionary<int, string> dictionary = new Dictionary<int, string>();

            foreach (int value in Enum.GetValues(enumType))
            {
                string? name = Enum.GetName(enumType, value);
                if (name is null)
                {
                    throw new InvalidOperationException($"Unable to find the name for the enum value '{value}'.");
                }

                dictionary.Add(value, name);
            }

            return dictionary;
        }

        private static string LookupResource(Type resourceManagerProvider, string resourceKey)
        {
            var resourceKeyProperty = resourceManagerProvider.GetProperty(resourceKey,
                BindingFlags.Static | BindingFlags.Public, null, typeof(string),
                new Type[0], null);

            if (resourceKeyProperty?.GetMethod != null)
            {
                var result = resourceKeyProperty.GetMethod.Invoke(null, null);
                if (result != null)
                {
                    return (string)result;
                }
            }

            return resourceKey ?? throw new InvalidOperationException("The resource key is null.");
        }
    }
}
