using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using Core.Helpers.Helpers;
using MediatR;
using System.Net;
using webAPI.Application.Features.Users.Rules;
using webAPI.Application.Services.Repositories;
using static webAPI.Application.Features.Users.Constants.UsersOperationClaims;

namespace webAPI.Application.Features.Users.Commands.Create
{
    public class CreateUserCommand : IRequest<CustomResponseDto<CreatedUserResponse>>, ISecuredRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public CreateUserCommand()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
        }

        public CreateUserCommand(string firstName, string lastName, string email, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }

        public string[] Roles => new[] { Admin, Write, Add };

        public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CustomResponseDto<CreatedUserResponse>>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;
            private readonly UserBusinessRules _userBusinessRules;

            public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper, UserBusinessRules userBusinessRules)
            {
                _userRepository = userRepository;
                _mapper = mapper;
                _userBusinessRules = userBusinessRules;
            }

            public async Task<CustomResponseDto<CreatedUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
            {
                await _userBusinessRules.UserEmailShouldNotExistsWhenInsert(request.Email);
                User user = _mapper.Map<User>(request);

                HashingHelper.CreatePasswordHash(
                    request.Password,
                    passwordHash: out byte[] passwordHash,
                    passwordSalt: out byte[] passwordSalt
                );
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                User createdUser = await _userRepository.AddAsync(user);

                CreatedUserResponse response = _mapper.Map<CreatedUserResponse>(createdUser);
                return CustomResponseDto<CreatedUserResponse>.Success((int)HttpStatusCode.OK, response, true);
            }
        }
    }
}
