namespace Core.ElasticSearch.Models
{
    public class ElasticSearchConfig
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
