using Core.Application.Responses;
using Core.Security.JWT;

namespace webAPI.Application.Features.Auth.Commands.Register
{
    public class RegisteredResponse : IResponse
    {
        public AccessToken AccessToken { get; set; }
        public Core.Domain.Entities.RefreshToken RefreshToken { get; set; }

        public RegisteredResponse()
        {
            AccessToken = null!;
            RefreshToken = null!;
        }

        public RegisteredResponse(AccessToken accessToken, Core.Domain.Entities.RefreshToken refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
