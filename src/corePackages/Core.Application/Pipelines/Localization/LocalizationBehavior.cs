using Core.Persistence.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Core.Application.Pipelines.Localization
{
    public class LocalizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CustomStringLocalizer _localizer;
        public LocalizationBehavior(IHttpContextAccessor httpContextAccessor, CustomStringLocalizer localizer)
        {
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            string userLang = _httpContextAccessor.HttpContext.Request.Headers["Accept-Language"].ToString();

            if (string.IsNullOrEmpty(userLang)) userLang = "en-US";

            string firstLang = userLang.Split(',').FirstOrDefault();
            string lang = "en-US";
            var userClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userClaim is not null)
                firstLang = _localizer.GetUserCulture(Guid.Parse(userClaim.Value));

            switch (firstLang)
            {
                case "TR":
                    lang = "tr-TR";
                    break;
                case "US":
                    lang = "en-US";
                    break;
                default:
                    break;
            }
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            _localizer.SetCulture(lang);

            return await next();
        }
    }
}
