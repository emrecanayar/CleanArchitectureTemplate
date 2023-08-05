namespace Core.Helpers.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceFirst(this string input, string search, string replacement)
        {
            int index = input.IndexOf(search);
            if (index < 0)
            {
                return input;
            }
            return input.Substring(0, index) + replacement + input.Substring(index + search.Length);
        }

        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        public static string Reverse(this string input)
        {
            char[] chars = input.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        public static string ToTitleCaseWords(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var words = input.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].ToTitleCase();
            }
            return string.Join(" ", words);
        }

        public static string TrimStart(this string input, char characterToTrim)
        {
            return input.TrimStart(new char[] { characterToTrim });
        }

        public static string TrimEnd(this string input, char characterToTrim)
        {
            return input.TrimEnd(new char[] { characterToTrim });
        }

        public static string Trim(this string input, char characterToTrim)
        {
            return input.Trim(new char[] { characterToTrim });
        }

        public static string Truncate(this string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            if (maxLength <= 0)
            {
                return string.Empty;
            }

            if (input.Length <= maxLength)
            {
                return input;
            }

            return input.Substring(0, maxLength);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}
