using System.Net;
using AutoMapper;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using Core.Helpers.Helpers;
using MediatR;
using webAPI.Application.Features.Users.Rules;
using webAPI.Application.Services.AuthService;
using webAPI.Application.Services.Repositories;

namespace webAPI.Application.Features.Users.Commands.UpdateFromAuth
{
    public class UpdateUserFromAuthCommand : IRequest<CustomResponseDto<UpdatedUserFromAuthResponse>>
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public string? NewPassword { get; set; }

        public UpdateUserFromAuthCommand()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Password = string.Empty;
        }

        public UpdateUserFromAuthCommand(Guid id, string firstName, string lastName, string password)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Password = password;
        }

        public class UpdateUserFromAuthCommandHandler : IRequestHandler<UpdateUserFromAuthCommand, CustomResponseDto<UpdatedUserFromAuthResponse>>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;
            private readonly UserBusinessRules _userBusinessRules;
            private readonly IAuthService _authService;

            public UpdateUserFromAuthCommandHandler(
                IUserRepository userRepository,
                IMapper mapper,
                UserBusinessRules userBusinessRules,
                IAuthService authService)
            {
                _userRepository = userRepository;
                _mapper = mapper;
                _userBusinessRules = userBusinessRules;
                _authService = authService;
            }

            public async Task<CustomResponseDto<UpdatedUserFromAuthResponse>> Handle(UpdateUserFromAuthCommand request, CancellationToken cancellationToken)
            {
                User? user = await _userRepository.GetAsync(predicate: u => u.Id == request.Id, cancellationToken: cancellationToken);
                await _userBusinessRules.UserShouldBeExistsWhenSelected(user);
                await _userBusinessRules.UserPasswordShouldBeMatched(user: user!, request.Password);
                await _userBusinessRules.UserEmailShouldNotExistsWhenUpdate(user!.Id, user.Email);

                user = _mapper.Map(request, user);
                if (request.NewPassword != null && !string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    HashingHelper.CreatePasswordHash(
                        request.Password,
                        passwordHash: out byte[] passwordHash,
                        passwordSalt: out byte[] passwordSalt);
                    user!.PasswordHash = passwordHash;
                    user!.PasswordSalt = passwordSalt;
                }

                User updatedUser = await _userRepository.UpdateAsync(user!);

                UpdatedUserFromAuthResponse response = _mapper.Map<UpdatedUserFromAuthResponse>(updatedUser);
                response.AccessToken = await _authService.CreateAccessToken(user!);
                return CustomResponseDto<UpdatedUserFromAuthResponse>.Success((int)HttpStatusCode.OK, response, true);
            }
        }
    }
}
