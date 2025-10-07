using Application.Features.Users.Queries.GetById;
using Core.Domain.ComplexTypes.Enums;
using Core.Security.JWT;

namespace Application.Features.Auth.Commands.Login;

public class LoggedResponse : IResponse
{
    public AccessToken? AccessToken { get; set; }

    public Core.Domain.Entities.RefreshToken? RefreshToken { get; set; }

    public AuthenticatorType? RequiredAuthenticatorType { get; set; }

    public GetByIdUserResponse User { get; set; }

    public LoggedHttpResponse ToHttpResponse() =>
        new()
        {
            AccessToken = AccessToken,
            RefreshToken = RefreshToken,
            User = User,
            RequiredAuthenticatorType = RequiredAuthenticatorType,
        };

    public class LoggedHttpResponse
    {
        public AccessToken? AccessToken { get; set; }

        public Core.Domain.Entities.RefreshToken? RefreshToken { get; set; }

        public GetByIdUserResponse User { get; set; }

        public AuthenticatorType? RequiredAuthenticatorType { get; set; }
    }
}
