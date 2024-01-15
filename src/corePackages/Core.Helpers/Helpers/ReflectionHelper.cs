using System.Reflection;

namespace Core.Helpers.Helpers
{
    /// <summary>
    /// Defines helper methods for reflection.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Checks whether <paramref name="givenType"/> implements/inherits <paramref name="genericType"/>.
        /// </summary>
        /// <param name="givenType">Type to check</param>
        /// <param name="genericType">Generic type</param>
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

        /// <summary>
        /// Gets a list of attributes defined for a class member and it's declaring type including inherited attributes.
        /// </summary>
        /// <param name="inherit">Inherit attribute from base classes</param>
        /// <param name="memberInfo">MemberInfo</param>
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


        /// <summary>
        /// Gets a list of attributes defined for a class member and it's declaring type including inherited attributes.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="memberInfo">MemberInfo</param>
        /// <param name="inherit">Inherit attribute from base classes</param>
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

        /// <summary>
        /// Gets a list of attributes defined for a class member and type including inherited attributes.
        /// </summary>
        /// <param name="memberInfo">MemberInfo</param>
        /// <param name="type">Type</param>
        /// <param name="inherit">Inherit attribute from base classes</param>
        public static List<object> GetAttributesOfMemberAndType(MemberInfo memberInfo, Type type, bool inherit = true)
        {
            var attributeList = new List<object>();
            attributeList.AddRange(memberInfo.GetCustomAttributes(inherit));
            attributeList.AddRange(type.GetTypeInfo().GetCustomAttributes(inherit));
            return attributeList;
        }


        /// <summary>
        /// Gets a list of attributes defined for a class member and type including inherited attributes.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="memberInfo">MemberInfo</param>
        /// <param name="type">Type</param>
        /// <param name="inherit">Inherit attribute from base classes</param>
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

        /// <summary>
        /// Tries to gets an of attribute defined for a class member and it's declaring type including inherited attributes.
        /// Returns default value if it's not declared at all.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="memberInfo">MemberInfo</param>
        /// <param name="defaultValue">Default value (null as default)</param>
        /// <param name="inherit">Inherit attribute from base classes</param>
        public static TAttribute? GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute? defaultValue = default)
         where TAttribute : class
        {
            return memberInfo.GetCustomAttributes(true).OfType<TAttribute>().FirstOrDefault()
                   ?? memberInfo.ReflectedType?.GetTypeInfo().GetCustomAttributes(true).OfType<TAttribute>().FirstOrDefault()
                   ?? defaultValue;
        }

        /// <summary>
        /// Tries to gets an of attribute defined for a class member and it's declaring type including inherited attributes.
        /// Returns default value if it's not declared at all.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="memberInfo">MemberInfo</param>
        /// <param name="defaultValue">Default value (null as default)</param>
        /// <param name="inherit">Inherit attribute from base classes</param>
        public static TAttribute? GetSingleAttributeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute? defaultValue = default, bool inherit = true)
          where TAttribute : class
        {
            //Get attribute on the member
            if (memberInfo.IsDefined(typeof(TAttribute), inherit))
            {
                return memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().First();
            }

            return defaultValue;
        }


        /// <summary>
        /// Gets a property by it's full path from given object
        /// </summary>
        /// <param name="obj">Object to get value from</param>
        /// <param name="objectType">Type of given object</param>
        /// <param name="propertyPath">Full path of property</param>
        /// <returns></returns>
        internal static PropertyInfo? GetPropertyByPath(Type objectType, string propertyPath)
        {
            PropertyInfo? property = null;
            Type currentType = objectType;
            string objectPath = currentType.FullName ?? string.Empty;
            string absolutePropertyPath = propertyPath;
            if (absolutePropertyPath.StartsWith(objectPath, StringComparison.OrdinalIgnoreCase))
            {
                absolutePropertyPath = absolutePropertyPath.Replace(objectPath + ".", "");
            }

            foreach (var propertyName in absolutePropertyPath.Split('.'))
            {
                property = currentType.GetProperty(propertyName);
                if (property == null)
                {
                    // Handle the case where the property doesn't exist
                    return null;
                }
                currentType = property.PropertyType;
            }

            return property;
        }


        /// <summary>
        /// Gets value of a property by it's full path from given object
        /// </summary>
        /// <param name="obj">Object to get value from</param>
        /// <param name="objectType">Type of given object</param>
        /// <param name="propertyPath">Full path of property</param>
        /// <returns></returns>
        internal static object? GetValueByPath(object obj, Type objectType, string propertyPath)
        {
            object? value = obj;
            Type? currentType = objectType;
            string? objectPath = currentType.FullName ?? string.Empty;
            var absolutePropertyPath = propertyPath;
            if (absolutePropertyPath.StartsWith(objectPath, StringComparison.Ordinal))
            {
                absolutePropertyPath = absolutePropertyPath.Replace(objectPath + ".", "");
            }

            foreach (var propertyName in absolutePropertyPath.Split('.'))
            {
                PropertyInfo? property = currentType?.GetProperty(propertyName);
                value = property?.GetValue(value, null);
                currentType = property?.PropertyType;
            }

            return value;
        }

        /// <summary>
        /// Sets value of a property by it's full path on given object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objectType"></param>
        /// <param name="propertyPath"></param>
        /// <param name="value"></param>
        internal static void SetValueByPath(object? obj, Type objectType, string propertyPath, object value)
        {
            var currentType = objectType;
            PropertyInfo? property;
            string? objectPath = currentType.FullName ?? string.Empty;
            var absolutePropertyPath = propertyPath;
            if (absolutePropertyPath.StartsWith(objectPath, StringComparison.Ordinal))
            {
                absolutePropertyPath = absolutePropertyPath.Replace(objectPath + ".", "");
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
            else return null;

        }


        public static List<string> GetNavigationPropertyNames(Type entityType)
        {
            var navigationPropertyNames = new List<string>();

            foreach (PropertyInfo property in entityType.GetProperties())
            {
                // Check for ICollection<T> type navigation properties
                if (property.PropertyType.IsGenericType &&
                    (property.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
                     || property.PropertyType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))))
                {
                    navigationPropertyNames.Add(property.Name);
                }
                // Check for single navigation properties
                else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    MethodInfo? getMethod = property.GetGetMethod();
                    bool isVirtual = getMethod != null && getMethod.IsVirtual;
                    if (isVirtual && property.PropertyType.Namespace == "System.Data.Entity.DynamicProxies")
                        continue; // Ignore EF dynamic proxy

                    navigationPropertyNames.Add(property.Name);
                }
            }

            return navigationPropertyNames;
        }

    }
}
