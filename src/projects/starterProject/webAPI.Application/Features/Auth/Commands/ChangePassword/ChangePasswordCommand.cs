using System.Net;
using System.Text.Json.Serialization;
using Application.Features.Auth.Rules;
using Application.Features.Users.Constants;
using Application.Services.AuthService;
using Application.Services.Repositories;
using Core.Application.Pipelines.Authorization;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using Core.Helpers.Helpers;
using MediatR;
using static Application.Features.Users.Constants.UsersOperationClaims;

namespace Application.Features.Auths.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest<CustomResponseDto<bool>>, ISecuredRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        public string[] Roles => new[] { Admin, UsersOperationClaims.Write, Add };

        public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, CustomResponseDto<bool>>
        {
            private readonly IAuthService _authService;
            private readonly IUserRepository _userRepository;
            private readonly AuthBusinessRules _authBusinessRules;

            public ChangePasswordCommandHandler(IAuthService authService, IUserRepository userRepository, AuthBusinessRules authBusinessRules)
            {
                this._authService = authService;
                this._userRepository = userRepository;
                this._authBusinessRules = authBusinessRules;
            }

            public async Task<CustomResponseDto<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
            {
                await _authBusinessRules.IsConfirmPasswordForChangePassword(request.NewPassword, request.ConfirmPassword);

                byte[] passwordHash, passwordSalt;

                HashingHelper.CreatePasswordHash(request.NewPassword, out passwordHash, out passwordSalt);

                User? user = await _userRepository.GetAsync(predicate: u => u.Id == request.UserId, enableTracking: false, cancellationToken: cancellationToken);
                await _authBusinessRules.UserShouldBeExistsWhenSelected(user);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                await _userRepository.UpdateAsync(user);

                return CustomResponseDto<bool>.Success((int)HttpStatusCode.OK, true, true);
            }
        }
    }
}
