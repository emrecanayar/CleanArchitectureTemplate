using Microsoft.Extensions.Configuration;

namespace Core.Helpers.Helpers
{
    public class RootFolderHelper
    {
        private readonly IConfiguration _configuration;

        public RootFolderHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string WebRootPath => _configuration["WebRootPath"];

        public string WebApiDomain => _configuration["WebApiDomain"];

        public string WebSiteDomain => _configuration["WebSiteDomain"];

        public string WebApiAdminDomain => _configuration["WebApiAdminDomain"];

        public string WebSiteAdminDomain => _configuration["WebSiteAdminDomain"];

        public string GetWebSiteUrl(string path, string pathParam = null)
            => pathParam is null ? $"{WebSiteDomain}/{path}" : $"{WebSiteDomain}/{path}/{pathParam}";

        public string GetWebApiUrl(string path, string pathParam = null)
            => pathParam is null ? $"{WebApiDomain}/{path}" : $"{WebApiDomain}/{path}/{pathParam}";

        public string GetWebSiteAdminUrl(string path, string pathParam = null)
            => pathParam is null ? $"{WebSiteAdminDomain}/{path}" : $"{WebSiteAdminDomain}/{path}/{pathParam}";


        public string GetWebApiAdminUrl(string path, string pathParam = null)
            => pathParam is null ? $"{WebApiAdminDomain}/{path}" : $"{WebApiAdminDomain}/{path}/{pathParam}";
    }
}
