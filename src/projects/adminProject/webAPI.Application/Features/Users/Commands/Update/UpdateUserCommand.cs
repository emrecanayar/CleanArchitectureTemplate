using System.Net;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using Core.Helpers.Helpers;
using MediatR;
using webAPI.Application.Features.Users.Constants;
using webAPI.Application.Features.Users.Rules;
using webAPI.Application.Services.Repositories;
using static webAPI.Application.Features.Users.Constants.UsersOperationClaims;

namespace webAPI.Application.Features.Users.Commands.Update
{
    public class UpdateUserCommand : IRequest<CustomResponseDto<UpdatedUserResponse>>, ISecuredRequest
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public UpdateUserCommand()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
        }

        public UpdateUserCommand(Guid id, string firstName, string lastName, string email, string password)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }

        public string[] Roles => new[] { Admin, Write, UsersOperationClaims.Update };

        public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, CustomResponseDto<UpdatedUserResponse>>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;
            private readonly UserBusinessRules _userBusinessRules;

            public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper, UserBusinessRules userBusinessRules)
            {
                _userRepository = userRepository;
                _mapper = mapper;
                _userBusinessRules = userBusinessRules;
            }

            public async Task<CustomResponseDto<UpdatedUserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                User? user = await _userRepository.GetAsync(predicate: u => u.Id == request.Id, cancellationToken: cancellationToken);
                await _userBusinessRules.UserShouldBeExistsWhenSelected(user);
                await _userBusinessRules.UserEmailShouldNotExistsWhenUpdate(user!.Id, user.Email);
                user = _mapper.Map(request, user);

                HashingHelper.CreatePasswordHash(
                    request.Password,
                    passwordHash: out byte[] passwordHash,
                    passwordSalt: out byte[] passwordSalt);
                user!.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                await _userRepository.UpdateAsync(user);

                UpdatedUserResponse response = _mapper.Map<UpdatedUserResponse>(user);
                return CustomResponseDto<UpdatedUserResponse>.Success((int)HttpStatusCode.OK, response, true);
            }
        }
    }
}
