using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using MediatR;
using System.Net;
using webAPI.Application.Features.UserOperationClaims.Rules;
using webAPI.Application.Services.Repositories;
using static webAPI.Application.Features.UserOperationClaims.Constants.UserOperationClaimsOperationClaims;

namespace webAPI.Application.Features.UserOperationClaims.Commands.Create
{
    public class CreateUserOperationClaimCommand : IRequest<CustomResponseDto<CreatedUserOperationClaimResponse>>, ISecuredRequest
    {
        public Guid UserId { get; set; }
        public Guid OperationClaimId { get; set; }

        public string[] Roles => new[] { Admin, Write, Add };

        public class CreateUserOperationClaimCommandHandler
            : IRequestHandler<CreateUserOperationClaimCommand, CustomResponseDto<CreatedUserOperationClaimResponse>>
        {
            private readonly IUserOperationClaimRepository _userOperationClaimRepository;
            private readonly IMapper _mapper;
            private readonly UserOperationClaimBusinessRules _userOperationClaimBusinessRules;

            public CreateUserOperationClaimCommandHandler(
                IUserOperationClaimRepository userOperationClaimRepository,
                IMapper mapper,
                UserOperationClaimBusinessRules userOperationClaimBusinessRules
            )
            {
                _userOperationClaimRepository = userOperationClaimRepository;
                _mapper = mapper;
                _userOperationClaimBusinessRules = userOperationClaimBusinessRules;
            }

            public async Task<CustomResponseDto<CreatedUserOperationClaimResponse>> Handle(
                CreateUserOperationClaimCommand request,
                CancellationToken cancellationToken
            )
            {
                await _userOperationClaimBusinessRules.UserShouldNotHasOperationClaimAlreadyWhenInsert(
                    request.UserId,
                    request.OperationClaimId
                );
                UserOperationClaim mappedUserOperationClaim = _mapper.Map<UserOperationClaim>(request);

                UserOperationClaim createdUserOperationClaim = await _userOperationClaimRepository.AddAsync(mappedUserOperationClaim);

                CreatedUserOperationClaimResponse createdUserOperationClaimDto = _mapper.Map<CreatedUserOperationClaimResponse>(
                    createdUserOperationClaim
                );
                return CustomResponseDto<CreatedUserOperationClaimResponse>.Success((int)HttpStatusCode.OK, createdUserOperationClaimDto, true);
            }
        }
    }
}
