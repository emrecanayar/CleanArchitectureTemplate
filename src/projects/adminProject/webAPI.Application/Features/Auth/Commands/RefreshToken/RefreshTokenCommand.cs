using System.Net;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using Core.Security.JWT;
using MediatR;
using webAPI.Application.Features.Auth.Rules;
using webAPI.Application.Services.AuthService;
using webAPI.Application.Services.UsersService;

namespace webAPI.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<CustomResponseDto<RefreshedTokensResponse>>
    {
        public string RefreshToken { get; set; }

        public string IpAddress { get; set; }

        public RefreshTokenCommand()
        {
            RefreshToken = string.Empty;
            IpAddress = string.Empty;
        }

        public RefreshTokenCommand(string refreshToken, string ipAddress)
        {
            RefreshToken = refreshToken;
            IpAddress = ipAddress;
        }

        public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, CustomResponseDto<RefreshedTokensResponse>>
        {
            private readonly IAuthService _authService;
            private readonly IUserService _userService;
            private readonly AuthBusinessRules _authBusinessRules;

            public RefreshTokenCommandHandler(IAuthService authService, IUserService userService, AuthBusinessRules authBusinessRules)
            {
                _authService = authService;
                _userService = userService;
                _authBusinessRules = authBusinessRules;
            }

            public async Task<CustomResponseDto<RefreshedTokensResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
            {
                Core.Domain.Entities.RefreshToken? refreshToken = await _authService.GetRefreshTokenByToken(request.RefreshToken);
                await _authBusinessRules.RefreshTokenShouldBeExists(refreshToken);

                if (refreshToken!.Revoked != null)
                {
                    await _authService.RevokeDescendantRefreshTokens(
                        refreshToken,
                        request.IpAddress,
                        reason: $"Attempted reuse of revoked ancestor token: {refreshToken.Token}");
                }

                await _authBusinessRules.RefreshTokenShouldBeActive(refreshToken);

                User? user = await _userService.GetAsync(predicate: u => u.Id == refreshToken.UserId, cancellationToken: cancellationToken);
                await _authBusinessRules.UserShouldBeExistsWhenSelected(user);

                Core.Domain.Entities.RefreshToken newRefreshToken = await _authService.RotateRefreshToken(
                    user: user!,
                    refreshToken,
                    request.IpAddress);
                Core.Domain.Entities.RefreshToken addedRefreshToken = await _authService.AddRefreshToken(newRefreshToken);
                await _authService.DeleteOldRefreshTokens(refreshToken.UserId);

                AccessToken createdAccessToken = await _authService.CreateAccessToken(user!);

                RefreshedTokensResponse refreshedTokensResponse = new() { AccessToken = createdAccessToken, RefreshToken = addedRefreshToken };
                return CustomResponseDto<RefreshedTokensResponse>.Success((int)HttpStatusCode.OK, refreshedTokensResponse, true);
            }
        }
    }
}
