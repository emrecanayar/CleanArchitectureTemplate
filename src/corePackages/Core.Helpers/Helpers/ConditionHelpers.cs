namespace Core.Helpers.Helpers
{
    public static class ConditionHelpers
    {
        public static bool ArrayIsNullOrEmpty<T>(T[] array)
        {
            if (array == null || array.Length == 0)
                return true;
            return false;
        }
    }
}
