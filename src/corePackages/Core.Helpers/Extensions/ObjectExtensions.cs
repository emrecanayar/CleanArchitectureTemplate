using System.ComponentModel;
using System.Globalization;

namespace Core.Helpers.Extensions
{
    public static class ObjectExtensions
    {
        public static T As<T>(this object obj)
            where T : class
        {
            return (T)obj;
        }

        public static T To<T>(this object obj)
            where T : struct
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

        public static bool IsIn<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }
    }
}
