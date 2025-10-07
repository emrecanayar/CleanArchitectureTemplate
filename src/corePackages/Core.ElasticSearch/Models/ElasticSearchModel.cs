using Nest;

namespace Core.ElasticSearch.Models
{
    public class ElasticSearchModel
    {
        public Id ElasticId { get; set; }

        public string IndexName { get; set; } = string.Empty;

        public ElasticSearchModel()
        {
            ElasticId = new Id(Guid.NewGuid().ToString());
        }
    }
}
