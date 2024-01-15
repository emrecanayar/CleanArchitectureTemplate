using System.ComponentModel;
using System.Globalization;

namespace Core.Helpers.Extensions
{
    /// <summary>
    /// Extension methods for all objects.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Used to simplify and beautify casting an object to a type. 
        /// </summary>
        /// <typeparam name="T">Type to be casted</typeparam>
        /// <param name="obj">Object to cast</param>
        /// <returns>Casted object</returns>
        public static T As<T>(this object obj)
            where T : class
        {
            return (T)obj;
        }

        /// <summary>
        /// Converts given object to a value or enum type using <see cref="Convert.ChangeType(object,TypeCode)"/> or <see cref="Enum.Parse(Type,string)"/> method.
        /// </summary>
        /// <param name="obj">Object to be converted</param>
        /// <typeparam name="T">Type of the target object</typeparam>
        /// <returns>Converted object</returns>
        public static T To<T>(this object obj) where T : struct
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (typeof(T) == typeof(Guid) || typeof(T) == typeof(TimeSpan))
            {
                var objString = obj.ToString();
                if (objString == null)
                {
                    throw new InvalidOperationException("Unable to convert the object to a string.");
                }

                var converter = TypeDescriptor.GetConverter(typeof(T));
                var result = converter.ConvertFromInvariantString(objString);
                if (result == null)
                {
                    throw new InvalidOperationException("Unable to convert the string to the target type.");
                }

                return (T)result;
            }

            if (typeof(T).IsEnum)
            {
                var objString = obj.ToString();
                if (objString == null)
                {
                    throw new InvalidOperationException("Unable to convert the object to a string.");
                }

                if (Enum.IsDefined(typeof(T), obj))
                {
                    return (T)Enum.Parse(typeof(T), objString);
                }
                else
                {
                    throw new ArgumentException($"Enum type undefined '{obj}'.");
                }
            }

            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// Check if an item is in a list.
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="list">List of items</param>
        /// <typeparam name="T">Type of the items</typeparam>
        public static bool IsIn<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }
    }
}
