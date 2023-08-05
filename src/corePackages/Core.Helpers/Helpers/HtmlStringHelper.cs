using System.Web;

namespace Core.Helpers.Helpers
{
    public static class HtmlStringHelper
    {
        public static string EncodeHtml(string html)
        {
            return HttpUtility.HtmlEncode(html);
        }

        public static string DecodeHtml(string html)
        {
            StringWriter myWriter = new StringWriter();
            HttpUtility.HtmlDecode(html, myWriter);
            string myDecodedString = myWriter.ToString();
            return myDecodedString;
        }

        public static string ReplaceValuesWithFlagsInHtml(string htmlContent, params string[] values)
        {
            var flagNo = 0;
            foreach (var value in values)
            {
                var flag = $"@{flagNo}";
                htmlContent = htmlContent.Replace(flag, value);
                flagNo++;
            }
            return htmlContent;
        }

        private static string aggregateQueryParams(object queryParams)
        {
            var dataStrings = new List<string>();
            foreach (var property in queryParams.GetType().GetProperties())
            {
                dataStrings.Add(property.Name);
                dataStrings.Add(property.GetValue(queryParams)?.ToString() ?? "");
            }
            var queryString = dataStrings.Aggregate((fItem, sItem) => $"{fItem}={sItem}&").TrimEnd('&');
            return queryString;
        }
    }
}
