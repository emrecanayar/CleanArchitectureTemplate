namespace Core.ElasticSearch.Models
{
    public class SearchParameters
    {
        public string IndexName { get; set; } = string.Empty;

        public int From { get; set; } = 0;

        public int Size { get; set; } = 10;
    }
}
