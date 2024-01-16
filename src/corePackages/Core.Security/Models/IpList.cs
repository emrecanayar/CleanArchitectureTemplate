namespace Core.Security.Models
{
    public class IpList
    {
        public string[] WhiteList { get; set; }
        public IpList()
        {
            WhiteList = Array.Empty<string>();
        }
    }
}
