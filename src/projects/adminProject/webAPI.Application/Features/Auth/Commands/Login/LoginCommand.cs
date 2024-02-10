using Core.Application.Dtos;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.ComplexTypes.Enums;
using Core.Domain.Entities;
using Core.Security.JWT;
using MediatR;
using System.Net;
using webAPI.Application.Features.Auth.Rules;
using webAPI.Application.Services.AuthenticatorService;
using webAPI.Application.Services.AuthService;
using webAPI.Application.Services.UsersService;

namespace webAPI.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<CustomResponseDto<LoggedResponse>>
    {
        public UserForLoginDto UserForLoginDto { get; set; }
        public string IpAddress { get; set; }

        public LoginCommand()
        {
            UserForLoginDto = null!;
            IpAddress = string.Empty;
        }

        public LoginCommand(UserForLoginDto userForLoginDto, string ipAddress)
        {
            UserForLoginDto = userForLoginDto;
            IpAddress = ipAddress;
        }

        public class LoginCommandHandler : IRequestHandler<LoginCommand, CustomResponseDto<LoggedResponse>>
        {
            private readonly AuthBusinessRules _authBusinessRules;
            private readonly IAuthenticatorService _authenticatorService;
            private readonly IAuthService _authService;
            private readonly IUserService _userService;

            public LoginCommandHandler(
                IUserService userService,
                IAuthService authService,
                AuthBusinessRules authBusinessRules,
                IAuthenticatorService authenticatorService
            )
            {
                _userService = userService;
                _authService = authService;
                _authBusinessRules = authBusinessRules;
                _authenticatorService = authenticatorService;
            }

            public async Task<CustomResponseDto<LoggedResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                User? user = await _userService.GetAsync(
                    predicate: u => u.Email == request.UserForLoginDto.Email,
                    cancellationToken: cancellationToken
                );
                await _authBusinessRules.UserShouldBeExistsWhenSelected(user);
                await _authBusinessRules.UserPasswordShouldBeMatch(user!.Id, request.UserForLoginDto.Password);

                LoggedResponse loggedResponse = new();

                if (user.AuthenticatorType is not AuthenticatorType.None)
                {
                    if (request.UserForLoginDto.AuthenticatorCode is null)
                    {
                        await _authenticatorService.SendAuthenticatorCode(user);
                        loggedResponse.RequiredAuthenticatorType = user.AuthenticatorType;
                        return CustomResponseDto<LoggedResponse>.Success((int)HttpStatusCode.OK, loggedResponse, true);
                    }

                    await _authenticatorService.VerifyAuthenticatorCode(user, request.UserForLoginDto.AuthenticatorCode);
                }

                AccessToken createdAccessToken = await _authService.CreateAccessToken(user);

                Core.Domain.Entities.RefreshToken createdRefreshToken = await _authService.CreateRefreshToken(user, request.IpAddress);
                Core.Domain.Entities.RefreshToken addedRefreshToken = await _authService.AddRefreshToken(createdRefreshToken);
                await _authService.DeleteOldRefreshTokens(user.Id);

                loggedResponse.AccessToken = createdAccessToken;
                loggedResponse.RefreshToken = addedRefreshToken;
                return CustomResponseDto<LoggedResponse>.Success((int)HttpStatusCode.OK, loggedResponse, true);
            }
        }
    }
}
