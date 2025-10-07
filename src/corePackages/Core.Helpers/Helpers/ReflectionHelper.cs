using System.Reflection;

namespace Core.Helpers.Helpers
{
    public static class ReflectionHelper
    {
        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var givenTypeInfo = givenType.GetTypeInfo();

            if (givenTypeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            foreach (var interfaceType in givenType.GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenTypeInfo.BaseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(givenTypeInfo.BaseType, genericType);
        }

        public static List<object> GetAttributesOfMemberAndDeclaringType(MemberInfo memberInfo, bool inherit = true)
        {
            var attributeList = new List<object>();

            attributeList.AddRange(memberInfo.GetCustomAttributes(inherit));

            if (memberInfo.DeclaringType != null)
            {
                attributeList.AddRange(memberInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(inherit));
            }

            return attributeList;
        }

        public static List<TAttribute> GetAttributesOfMemberAndDeclaringType<TAttribute>(MemberInfo memberInfo, bool inherit = true)
       where TAttribute : Attribute
        {
            var attributeList = new List<TAttribute>();

            if (memberInfo.IsDefined(typeof(TAttribute), inherit))
            {
                attributeList.AddRange(memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>());
            }

            if (memberInfo.DeclaringType != null && memberInfo.DeclaringType.GetTypeInfo().IsDefined(typeof(TAttribute), inherit))
            {
                attributeList.AddRange(memberInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>());
            }

            return attributeList;
        }

        public static List<object> GetAttributesOfMemberAndType(MemberInfo memberInfo, Type type, bool inherit = true)
        {
            var attributeList = new List<object>();
            attributeList.AddRange(memberInfo.GetCustomAttributes(inherit));
            attributeList.AddRange(type.GetTypeInfo().GetCustomAttributes(inherit));
            return attributeList;
        }

        public static List<TAttribute> GetAttributesOfMemberAndType<TAttribute>(MemberInfo memberInfo, Type type, bool inherit = true)
            where TAttribute : Attribute
        {
            var attributeList = new List<TAttribute>();

            if (memberInfo.IsDefined(typeof(TAttribute), inherit))
            {
                attributeList.AddRange(memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>());
            }

            if (type.GetTypeInfo().IsDefined(typeof(TAttribute), inherit))
            {
                attributeList.AddRange(type.GetTypeInfo().GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>());
            }

            return attributeList;
        }

        public static TAttribute? GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute? defaultValue = default)
         where TAttribute : class
        {
            return memberInfo.GetCustomAttributes(true).OfType<TAttribute>().FirstOrDefault()
                   ?? memberInfo.ReflectedType?.GetTypeInfo().GetCustomAttributes(true).OfType<TAttribute>().FirstOrDefault()
                   ?? defaultValue;
        }

        public static TAttribute? GetSingleAttributeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute? defaultValue = default, bool inherit = true)
          where TAttribute : class
        {
            if (memberInfo.IsDefined(typeof(TAttribute), inherit))
            {
                return memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().First();
            }

            return defaultValue;
        }

        public static List<string> GetNavigationPropertyNames(Type entityType)
        {
            var navigationPropertyNames = new List<string>();

            foreach (PropertyInfo property in entityType.GetProperties())
            {
                if (property.PropertyType.IsGenericType &&
                    (property.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
                     || property.PropertyType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))))
                {
                    navigationPropertyNames.Add(property.Name);
                }
                else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    MethodInfo? getMethod = property.GetGetMethod();
                    bool isVirtual = getMethod != null && getMethod.IsVirtual;
                    if (isVirtual && property.PropertyType.Namespace == "System.Data.Entity.DynamicProxies")
                    {
                        continue;
                    }

                    navigationPropertyNames.Add(property.Name);
                }
            }

            return navigationPropertyNames;
        }

        internal static PropertyInfo? GetPropertyByPath(Type objectType, string propertyPath)
        {
            PropertyInfo? property = null;
            Type currentType = objectType;
            string objectPath = currentType.FullName ?? string.Empty;
            string absolutePropertyPath = propertyPath;
            if (absolutePropertyPath.StartsWith(objectPath, StringComparison.OrdinalIgnoreCase))
            {
                absolutePropertyPath = absolutePropertyPath.Replace(objectPath + ".", string.Empty);
            }

            foreach (var propertyName in absolutePropertyPath.Split('.'))
            {
                property = currentType.GetProperty(propertyName);
                if (property == null)
                {
                    return null;
                }

                currentType = property.PropertyType;
            }

            return property;
        }

        internal static object? GetValueByPath(object obj, Type objectType, string propertyPath)
        {
            object? value = obj;
            Type? currentType = objectType;
            string? objectPath = currentType.FullName ?? string.Empty;
            var absolutePropertyPath = propertyPath;
            if (absolutePropertyPath.StartsWith(objectPath, StringComparison.Ordinal))
            {
                absolutePropertyPath = absolutePropertyPath.Replace(objectPath + ".", string.Empty);
            }

            foreach (var propertyName in absolutePropertyPath.Split('.'))
            {
                PropertyInfo? property = currentType?.GetProperty(propertyName);
                value = property?.GetValue(value, null);
                currentType = property?.PropertyType;
            }

            return value;
        }

        internal static void SetValueByPath(object? obj, Type objectType, string propertyPath, object value)
        {
            var currentType = objectType;
            PropertyInfo? property;
            string? objectPath = currentType.FullName ?? string.Empty;
            var absolutePropertyPath = propertyPath;
            if (absolutePropertyPath.StartsWith(objectPath, StringComparison.Ordinal))
            {
                absolutePropertyPath = absolutePropertyPath.Replace(objectPath + ".", string.Empty);
            }

            var properties = absolutePropertyPath.Split('.');

            if (properties.Length == 1)
            {
                property = objectType.GetProperty(properties[0]);
                property?.SetValue(obj, value);
                return;
            }

            for (int i = 0; i < properties.Length - 1; i++)
            {
                property = currentType?.GetProperty(properties[i]);
                obj = property?.GetValue(obj, null);
                currentType = property?.PropertyType;
            }

            property = currentType?.GetProperty(properties[^1]);
            property?.SetValue(obj, value);
        }

        internal static bool IsPropertyGetterSetterMethod(MethodInfo method, Type type)
        {
            if (!method.IsSpecialName)
            {
                return false;
            }

            if (method.Name.Length < 5)
            {
                return false;
            }

            return type?.GetProperty(method.Name[4..], bindingAttr: BindingFlags.Instance | BindingFlags.Static) != null;
        }

        internal static async Task<object?> InvokeAsync(MethodInfo method, object obj, params object[] parameters)
        {
            var result = method.Invoke(obj, parameters);
            if (result is Task task)
            {
                await task;
                var resultProperty = task.GetType().GetProperty("Result");
                return resultProperty?.GetValue(task);
            }
            else
            {
                return null;
            }
        }
    }
}
