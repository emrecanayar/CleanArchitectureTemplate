namespace Core.Helpers.Helpers
{
    public static class Mapper
    {
        #region Source Type Methods

        public static IEnumerable<TDestination> ToMap<TSource, TDestination>(this IEnumerable<TSource> sourceList)
        {
            return sourceList.Select(source => source.ToMap<TSource, TDestination>());
        }

        public static List<TDestination> ToMapList<TSource, TDestination>(this IEnumerable<TSource> sourceList)
        {
            return sourceList.Select(source => source.ToMap<TSource, TDestination>()).ToList();
        }

        public static TDestination ToMap<TSource, TDestination>(this TSource source)
        {
            if (source == null) return default;
            var destinationInstance = NewInstanceMap<TDestination>(source);
            return destinationInstance;
        }

        public static TDestination ToMap<TSource, TDestination>(this TSource source, TDestination destination)
        {
            if (source == null) return default;
            var destinationInstance = ValuesUpdateMap(source, destination);
            return destinationInstance;
        }

        #endregion Source Type Methods

        #region Source Typeless Methods

        public static IEnumerable<TDestination> ToMap<TDestination>(this IEnumerable<object> sourceList)
        {
            return sourceList.Select(source => source.ToMap<TDestination>());
        }

        public static List<TDestination> ToMapList<TDestination>(this IEnumerable<object> sourceList)
        {
            return sourceList.Select(source => source.ToMap<TDestination>()).ToList();
        }

        public static TDestination ToMap<TDestination>(this object source)
        {
            if (source == null) return default;
            var destinationInstance = NewInstanceMap<TDestination>(source);
            return destinationInstance;
        }

        public static TDestination ToMap<TDestination>(this object source, TDestination destination)
        {
            if (source == null) return default;
            var destinationInstance = ValuesUpdateMap(source, destination);
            return destinationInstance;
        }

        #endregion Source Typeless Methods

        #region Private Methods

        private static TDestination NewInstanceMap<TDestination>(object source)
        {
            var entityProperties = source.GetType().GetProperties();
            var destinationInstance = GetInstanceByNameOrType<TDestination>(source.GetType().AssemblyQualifiedName);
            for (int i = 0; i < entityProperties.Length; i++)
            {
                var currentPropertyName = entityProperties[i].Name;
                var value = GetPropertyValue(source, currentPropertyName);
                if (value is null)
                    continue;
                if (destinationInstance.GetType().GetProperty(currentPropertyName) == null)
                    continue;
                var destProp = destinationInstance.GetType().GetProperty(currentPropertyName);
                if (destProp == null)
                    continue;
                try { destProp.SetValue(destinationInstance, value); }
                catch (Exception ex) { /* Uyuşmayan veri tipleri */ }
            }
            return destinationInstance;
        }

        private static TDestination ValuesUpdateMap<TDestination>(object source, TDestination destinationInstance)
        {
            var newDestinationInstance = GetInstanceByName<TDestination>(destinationInstance.GetType().AssemblyQualifiedName);
            newDestinationInstance = destinationInstance.ToMap<TDestination>();
            var entityProperties = source.GetType().GetProperties();
            for (int i = 0; i < entityProperties.Length; i++)
            {
                var currentPropertyName = entityProperties[i].Name;
                var value = GetPropertyValue(source, currentPropertyName);
                if (value is null)
                    continue;
                if (newDestinationInstance.GetType().GetProperty(currentPropertyName) == null)
                    continue;
                var destProp = newDestinationInstance.GetType().GetProperty(currentPropertyName);
                if (destProp == null)
                    continue;
                if (IsDefaultValue(value))
                    continue;
                try { destProp.SetValue(newDestinationInstance, value); }
                catch (Exception ex) { /* Uyuşmayan veri tipleri */ }
            }
            return newDestinationInstance;
        }

        private static object GetPropertyValue(object source, string propName)
        {
            if (source == null) throw new ArgumentException("Value cannot be null.", nameof(source));
            if (propName == null) throw new ArgumentException("Value cannot be null.", nameof(propName));
            var prop = source.GetType().GetProperty(propName);
            return prop != null ? prop.GetValue(source, null) : null;
        }

        private static bool IsDefaultValue(object value)
        {
            if (value.GetType() == typeof(string))
                return value.ToString() == string.Empty;
            try // for : System.MissingMethodException: 'No parameterless constructor defined for type 'Microsoft.AspNetCore.Http.FormFile'.'
            {
                return Activator.CreateInstance(value.GetType(), true).Equals(value);
            }
            catch (MissingMethodException) { return true; }
        }

        #endregion Private Methods

        #region Object Instance Helper Methods

        public static TType GetInstanceByNameOrType<TType>(string entityQualifiedName)
        {
            var destinationInstance = GetInstanceByType<TType>();
            if (destinationInstance.GetType() == typeof(object))
                destinationInstance = GetInstanceByName<TType>(entityQualifiedName);
            return destinationInstance;
        }

        public static TType GetInstanceByType<TType>()
        {
            return Activator.CreateInstance<TType>();
        }

        public static TType GetInstanceByName<TType>(string entityQualifiedName)
        {
            return (TType)Activator.CreateInstance(Type.GetType(entityQualifiedName));
        }

        public static object GetInstanceByName(string entityQualifiedName)
        {
            return Activator.CreateInstance(Type.GetType(entityQualifiedName));
        }

        #endregion Object Instance Helper Methods
    }
}