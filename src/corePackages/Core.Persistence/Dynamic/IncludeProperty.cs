namespace Core.Persistence.Dynamic
{
    public class IncludeProperty
    {
        public List<string> IncludeProperties { get; set; }

        public IncludeProperty()
        {
            IncludeProperties = new List<string>();
        }
    }
}
