using System.ComponentModel;

namespace Core.Helpers.Extensions
{
    public static class EnumExtensions
    {
        public static int GetEnumIndex<TEnum>(this TEnum @enum)
            where TEnum : struct, Enum
        {
            Array values = typeof(TEnum).GetEnumValues();
            return Array.IndexOf(values, @enum);
        }

        public static string? GetEnumName<TEnum>(this TEnum @enum)
            where TEnum : struct, Enum
        {
            var enumText = Enum.GetName(typeof(TEnum), @enum);
            return enumText;
        }

        public static object? GetEnumCustomAttributeValue<TEnum>(this TEnum @enum, Type customAttributeType)
            where TEnum : struct, Enum
        {
            var enumName = @enum.GetEnumName();
            if (enumName == null)
            {
                throw new InvalidOperationException("Unable to get the name of the enum.");
            }

            var enumMember = typeof(TEnum).GetMember(enumName).FirstOrDefault();
            if (enumMember == null)
            {
                return default;
            }

            var customAttribute = enumMember.GetCustomAttributesData().FirstOrDefault(w => w.AttributeType.Name == customAttributeType.Name);
            if (customAttribute == null)
            {
                return default;
            }

            var enumAttributeValue = customAttribute.NamedArguments.FirstOrDefault().TypedValue.Value;
            return enumAttributeValue;
        }

        public static object? GetEnumFirstCustomAttributeValue<TEnum>(this TEnum @enum)
            where TEnum : struct, Enum
        {
            var enumName = @enum.GetEnumName();
            if (enumName == null)
            {
                throw new InvalidOperationException("Unable to get the name of the enum.");
            }

            var enumMember = typeof(TEnum).GetMember(enumName).FirstOrDefault();
            if (enumMember == null)
            {
                return default;
            }

            var customAttribute = enumMember.GetCustomAttributesData().FirstOrDefault();
            if (customAttribute == null)
            {
                return default;
            }

            var enumAttributeValue = customAttribute.NamedArguments.FirstOrDefault().TypedValue.Value;
            return enumAttributeValue;
        }

        public static object[] GetEnumCustomAttributesValues<TEnum>(this TEnum @enum, params Type[] customAttributesTypes)
            where TEnum : struct, Enum
        {
            List<object> values = new List<object>();
            foreach (var customAttributeType in customAttributesTypes)
            {
                var attributeValue = @enum.GetEnumCustomAttributeValue(customAttributeType);
                if (attributeValue != null)
                {
                    values.Add(attributeValue);
                }
            }

            object[] array = [.. values];
            values.Clear();
            return array;
        }

        public static object?[] GetEnumAllCustomAttributesValues<TEnum>(this TEnum @enum)
            where TEnum : struct, Enum
        {
            var enumName = @enum.GetEnumName();
            if (enumName == null)
            {
                throw new InvalidOperationException("Unable to get the name of the enum.");
            }

            var enumMember = typeof(TEnum).GetMember(enumName).FirstOrDefault();
            if (enumMember == null)
            {
                return Array.Empty<object>();
            }

            var customAttributes = enumMember.GetCustomAttributesData();
            if (customAttributes == null || customAttributes.Count <= 0)
            {
                return Array.Empty<object>();
            }

            var values = customAttributes.Select(s => s.NamedArguments.FirstOrDefault()).Select(s => s.TypedValue.Value).ToList();
            return values.ToArray();
        }

        public static List<KeyValuePair<string, TValue>> GetEnumMemberNamesAndValues<TValue>(this Type enumType)
        {
            var dict = new Dictionary<string, TValue>();
            foreach (TValue value in Enum.GetValues(enumType))
            {
                var name = Enum.GetName(enumType, value);
                if (name == null)
                {
                    throw new InvalidOperationException($"Unable to find the name for the enum value '{value}'.");
                }

                dict.Add(name, value);
            }

            return [.. dict];
        }

        public static List<KeyValuePair<string, int>> GetEnumMemberNamesAndIntValues(this Type enumType)
        {
            var dict = new Dictionary<string, int>();
            foreach (int value in Enum.GetValues(enumType))
            {
                var name = Enum.GetName(enumType, value);
                if (name == null)
                {
                    throw new InvalidOperationException($"Unable to find the name for the enum value '{value}'.");
                }

                dict.Add(name, value);
            }

            return [.. dict];
        }

        public static string? GetDescription<T>(this T enumValue)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                return null;
            }

            var description = enumValue.ToString();
            if (description == null)
            {
                throw new InvalidOperationException("Unable to convert the enum value to a string.");
            }

            var fieldInfo = enumValue.GetType().GetField(description);

            if (fieldInfo == null)
            {
                return description;
            }

            var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);

            if (attrs != null && attrs.Length > 0)
            {
                description = ((DescriptionAttribute)attrs[0]).Description;
            }

            return description;
        }
    }
}
