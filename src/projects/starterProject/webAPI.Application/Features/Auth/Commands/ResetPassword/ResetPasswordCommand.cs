using System.Net;
using Application.Features.Auth.Rules;
using Application.Features.Users.Constants;
using Application.Services.AuthService;
using Application.Services.Repositories;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using Core.Helpers.Helpers;
using MediatR;
using static Application.Features.Users.Constants.UsersOperationClaims;

namespace Application.Features.Auths.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest<CustomResponseDto<bool>>
    {
        public Guid UserId { get; set; }

        public string Password { get; set; }

        public string[] Roles => new[] { Admin, UsersOperationClaims.Write, Add };

        public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, CustomResponseDto<bool>>
        {
            private readonly IAuthService _authService;
            private readonly IUserRepository _userRepository;
            private readonly AuthBusinessRules _authBusinessRules;

            public ResetPasswordCommandHandler(IAuthService authService, IUserRepository userRepository, AuthBusinessRules authBusinessRules)
            {
                this._authService = authService;
                this._userRepository = userRepository;
                this._authBusinessRules = authBusinessRules;
            }

            public async Task<CustomResponseDto<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
            {
                byte[] passwordHash, passwordSalt;

                User? user = await _userRepository.GetAsync(predicate: u => u.Id == request.UserId, enableTracking: false, cancellationToken: cancellationToken);
                await _authBusinessRules.UserShouldBeExistsWhenSelected(user);

                HashingHelper.CreatePasswordHash(request.Password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                await _userRepository.UpdateAsync(user);

                return CustomResponseDto<bool>.Success((int)HttpStatusCode.OK, true, true);
            }
        }
    }
}
