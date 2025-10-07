namespace Core.ElasticSearch.Models
{
    public class SearchByQueryParameters : SearchParameters
    {
        public string QueryName { get; set; } = string.Empty;

        public string Query { get; set; } = string.Empty;

        public string[] Fields { get; set; }

        public SearchByQueryParameters()
        {
            Fields = Array.Empty<string>();
        }
    }
}
