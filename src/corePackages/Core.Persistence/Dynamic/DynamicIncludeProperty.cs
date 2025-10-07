namespace Core.Persistence.Dynamic
{
    public class DynamicIncludeProperty
    {
        public DynamicQuery? Dynamic { get; set; }

        public List<string>? IncludeProperties { get; set; }

        public DynamicIncludeProperty()
        {
            IncludeProperties = new List<string>();
        }
    }
}
