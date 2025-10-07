using Core.Application.Responses;
using Core.Security.JWT;

namespace webAPI.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshedTokensResponse : IResponse
    {
        public AccessToken AccessToken { get; set; }

        public Core.Domain.Entities.RefreshToken RefreshToken { get; set; }

        public RefreshedTokensResponse()
        {
            AccessToken = null!;
            RefreshToken = null!;
        }

        public RefreshedTokensResponse(AccessToken accessToken, Core.Domain.Entities.RefreshToken refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
