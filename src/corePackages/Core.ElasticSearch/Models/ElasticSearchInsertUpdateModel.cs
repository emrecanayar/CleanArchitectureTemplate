namespace Core.ElasticSearch.Models
{
    public class ElasticSearchInsertUpdateModel : ElasticSearchModel
    {
        public object Item { get; set; }

        public ElasticSearchInsertUpdateModel()
        {
            Item = new object();
        }
    }
}
