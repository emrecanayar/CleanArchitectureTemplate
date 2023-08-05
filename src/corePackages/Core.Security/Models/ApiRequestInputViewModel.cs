namespace Core.Security.Models
{
    public class ApiRequestInputViewModel
    {
        public string RequestName { get; set; }
        public string RequestIP { get; set; }
        public string RequestUrl { get; set; }
        public string HttpType { get; set; }
        public string Query { get; set; }
        public string Body { get; set; }
        public string RequestTime { get; set; }
        public string ResponseBody { get; set; }
        public long ElapsedTime { get; set; }

        public ApiRequestInputViewModel()
        {
            this.RequestName = string.Empty;
            this.RequestIP = string.Empty;
            this.RequestUrl = string.Empty;
            this.HttpType = string.Empty;
            this.Query = string.Empty;
            this.RequestTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            this.Body = string.Empty;
            this.ResponseBody = string.Empty;
            this.ElapsedTime = -1;
        }
    }
}