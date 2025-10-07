namespace Core.ElasticSearch.Models
{
    public class IndexModel
    {
        public string IndexName { get; set; } = string.Empty;

        public string AliasName { get; set; } = string.Empty;

        public int NumberOfReplicas { get; set; } = 3;

        public int NumberOfShards { get; set; } = 3;
    }
}
