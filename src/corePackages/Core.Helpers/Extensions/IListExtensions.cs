namespace Core.Helpers.Extensions
{
    public static class IListExtensions
    {
        public static void ForEach<T>(this IList<T> values, Action<T> doOnValue)
        {
            if (values is IList<T> list)
            {
                int count = list.Count;
                for (int position = 0; position < count; position++)
                {
                    doOnValue?.Invoke(list[position]);
                }
            }
            else
            {
                foreach (var value in values)
                {
                    doOnValue?.Invoke(value);
                }
            }
        }
    }
}
