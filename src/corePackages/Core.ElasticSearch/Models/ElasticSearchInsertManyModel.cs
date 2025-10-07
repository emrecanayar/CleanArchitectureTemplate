namespace Core.ElasticSearch.Models
{
    public class ElasticSearchInsertManyModel : ElasticSearchModel
    {
        public object[] Items { get; set; }

        public ElasticSearchInsertManyModel()
        {
            Items = new object[0];
        }
    }
}
