namespace Core.Security.Limits
{
    public interface ILimitedRequest
    {
        public string Name { get; }

        public int NoOfRequest { get; set; }

        public int Seconds { get; set; }
    }
}