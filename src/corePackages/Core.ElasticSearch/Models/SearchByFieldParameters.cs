namespace Core.ElasticSearch.Models
{
    public class SearchByFieldParameters : SearchParameters
    {
        public string FieldName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
