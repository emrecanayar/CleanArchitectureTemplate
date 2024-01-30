namespace Core.ElasticSearch.Models
{
    public class ElasticSearchGetModel<T>
    {
        public string ElasticId { get; set; } = string.Empty;
        public T Item { get; set; }

        public ElasticSearchGetModel()
        {
            Item = default!;
        }
    }
}
