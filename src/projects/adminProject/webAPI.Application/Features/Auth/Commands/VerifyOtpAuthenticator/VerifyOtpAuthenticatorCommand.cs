using Core.Domain.ComplexTypes.Enums;
using Core.Domain.Entities;
using MediatR;
using webAPI.Application.Features.Auth.Rules;
using webAPI.Application.Services.AuthenticatorService;
using webAPI.Application.Services.Repositories;
using webAPI.Application.Services.UsersService;

namespace webAPI.Application.Features.Auth.Commands.VerifyOtpAuthenticator
{
    public class VerifyOtpAuthenticatorCommand : IRequest
    {
        public Guid UserId { get; set; }

        public string ActivationCode { get; set; }

        public VerifyOtpAuthenticatorCommand()
        {
            ActivationCode = string.Empty;
        }

        public VerifyOtpAuthenticatorCommand(Guid userId, string activationCode)
        {
            UserId = userId;
            ActivationCode = activationCode;
        }

        public class VerifyOtpAuthenticatorCommandHandler : IRequestHandler<VerifyOtpAuthenticatorCommand>
        {
            private readonly AuthBusinessRules _authBusinessRules;
            private readonly IAuthenticatorService _authenticatorService;
            private readonly IOtpAuthenticatorRepository _otpAuthenticatorRepository;
            private readonly IUserService _userService;

            public VerifyOtpAuthenticatorCommandHandler(
                IOtpAuthenticatorRepository otpAuthenticatorRepository,
                AuthBusinessRules authBusinessRules,
                IUserService userService,
                IAuthenticatorService authenticatorService)
            {
                _otpAuthenticatorRepository = otpAuthenticatorRepository;
                _authBusinessRules = authBusinessRules;
                _userService = userService;
                _authenticatorService = authenticatorService;
            }

            public async Task Handle(VerifyOtpAuthenticatorCommand request, CancellationToken cancellationToken)
            {
                OtpAuthenticator? otpAuthenticator = await _otpAuthenticatorRepository.GetAsync(
                    predicate: e => e.UserId == request.UserId,
                    cancellationToken: cancellationToken);
                await _authBusinessRules.OtpAuthenticatorShouldBeExists(otpAuthenticator);

                User? user = await _userService.GetAsync(predicate: u => u.Id == request.UserId, cancellationToken: cancellationToken);
                await _authBusinessRules.UserShouldBeExistsWhenSelected(user);

                otpAuthenticator!.IsVerified = true;
                user!.AuthenticatorType = AuthenticatorType.Otp;

                await _authenticatorService.VerifyAuthenticatorCode(user, request.ActivationCode);

                await _otpAuthenticatorRepository.UpdateAsync(otpAuthenticator);
                await _userService.UpdateAsync(user);
            }
        }
    }
}
