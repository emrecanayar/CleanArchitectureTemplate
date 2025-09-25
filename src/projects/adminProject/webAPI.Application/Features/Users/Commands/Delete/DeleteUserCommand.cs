using System.Net;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using MediatR;
using webAPI.Application.Features.Users.Constants;
using webAPI.Application.Features.Users.Rules;
using webAPI.Application.Services.Repositories;
using static webAPI.Application.Features.Users.Constants.UsersOperationClaims;


namespace webAPI.Application.Features.Users.Commands.Delete
{
    public class DeleteUserCommand : IRequest<CustomResponseDto<DeletedUserResponse>>, ISecuredRequest
    {
        public Guid Id { get; set; }

        public string[] Roles => new[] { Admin, Write, UsersOperationClaims.Delete };

        public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, CustomResponseDto<DeletedUserResponse>>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;
            private readonly UserBusinessRules _userBusinessRules;

            public DeleteUserCommandHandler(IUserRepository userRepository, IMapper mapper, UserBusinessRules userBusinessRules)
            {
                _userRepository = userRepository;
                _mapper = mapper;
                _userBusinessRules = userBusinessRules;
            }

            public async Task<CustomResponseDto<DeletedUserResponse>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
            {
                User? user = await _userRepository.GetAsync(predicate: u => u.Id == request.Id, cancellationToken: cancellationToken);
                await _userBusinessRules.UserShouldBeExistsWhenSelected(user);

                await _userRepository.DeleteAsync(user!);

                DeletedUserResponse response = _mapper.Map<DeletedUserResponse>(user);
                return CustomResponseDto<DeletedUserResponse>.Success((int)HttpStatusCode.OK, response, true);
            }
        }
    }
}
