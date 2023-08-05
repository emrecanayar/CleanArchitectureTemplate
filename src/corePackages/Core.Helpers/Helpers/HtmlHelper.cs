using System.Web;

namespace Core.Helpers.Helpers
{
    public static class HtmlHelper
    {
        public static string HtmlEncode(this string htmlString)
        {
            return HttpUtility.HtmlEncode(htmlString);
        }

        public static string HtmlDecode(this string htmlString)
        {
            return HttpUtility.HtmlDecode(htmlString);
        }
    }
}
