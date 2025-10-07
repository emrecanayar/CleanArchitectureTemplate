using System.Net;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using MediatR;
using webAPI.Application.Features.UserOperationClaims.Constants;
using webAPI.Application.Features.UserOperationClaims.Rules;
using webAPI.Application.Services.Repositories;
using static webAPI.Application.Features.UserOperationClaims.Constants.UserOperationClaimsOperationClaims;

namespace webAPI.Application.Features.UserOperationClaims.Commands.Delete
{
    public class DeleteUserOperationClaimCommand : IRequest<CustomResponseDto<DeletedUserOperationClaimResponse>>, ISecuredRequest
    {
        public Guid Id { get; set; }

        public string[] Roles => new[] { Admin, Write, UserOperationClaimsOperationClaims.Delete };

        public class DeleteUserOperationClaimCommandHandler
            : IRequestHandler<DeleteUserOperationClaimCommand, CustomResponseDto<DeletedUserOperationClaimResponse>>
        {
            private readonly IUserOperationClaimRepository _userOperationClaimRepository;
            private readonly IMapper _mapper;
            private readonly UserOperationClaimBusinessRules _userOperationClaimBusinessRules;

            public DeleteUserOperationClaimCommandHandler(
                IUserOperationClaimRepository userOperationClaimRepository,
                IMapper mapper,
                UserOperationClaimBusinessRules userOperationClaimBusinessRules)
            {
                _userOperationClaimRepository = userOperationClaimRepository;
                _mapper = mapper;
                _userOperationClaimBusinessRules = userOperationClaimBusinessRules;
            }

            public async Task<CustomResponseDto<DeletedUserOperationClaimResponse>> Handle(
                DeleteUserOperationClaimCommand request,
                CancellationToken cancellationToken)
            {
                UserOperationClaim? userOperationClaim = await _userOperationClaimRepository.GetAsync(
                    predicate: uoc => uoc.Id == request.Id,
                    cancellationToken: cancellationToken);
                await _userOperationClaimBusinessRules.UserOperationClaimShouldExistWhenSelected(userOperationClaim);

                await _userOperationClaimRepository.DeleteAsync(userOperationClaim!);

                DeletedUserOperationClaimResponse response = _mapper.Map<DeletedUserOperationClaimResponse>(userOperationClaim);
                return CustomResponseDto<DeletedUserOperationClaimResponse>.Success((int)HttpStatusCode.OK, response, true);
            }
        }
    }
}
