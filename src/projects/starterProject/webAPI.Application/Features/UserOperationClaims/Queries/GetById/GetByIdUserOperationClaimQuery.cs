using System.Net;
using Application.Features.UserOperationClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using MediatR;

namespace Application.Features.UserOperationClaims.Queries.GetById;

public class GetByIdUserOperationClaimQuery : IRequest<CustomResponseDto<GetByIdUserOperationClaimResponse>>
{
    public Guid Id { get; set; }

    public class GetByIdUserOperationClaimQueryHandler : IRequestHandler<GetByIdUserOperationClaimQuery, CustomResponseDto<GetByIdUserOperationClaimResponse>>
    {
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;
        private readonly IMapper _mapper;
        private readonly UserOperationClaimBusinessRules _userOperationClaimBusinessRules;

        public GetByIdUserOperationClaimQueryHandler(
            IUserOperationClaimRepository userOperationClaimRepository,
            IMapper mapper,
            UserOperationClaimBusinessRules userOperationClaimBusinessRules)
        {
            _userOperationClaimRepository = userOperationClaimRepository;
            _mapper = mapper;
            _userOperationClaimBusinessRules = userOperationClaimBusinessRules;
        }

        public async Task<CustomResponseDto<GetByIdUserOperationClaimResponse>> Handle(
            GetByIdUserOperationClaimQuery request,
            CancellationToken cancellationToken)
        {
            UserOperationClaim? userOperationClaim = await _userOperationClaimRepository.GetAsync(
                predicate: b => b.Id == request.Id,
                cancellationToken: cancellationToken);
            await _userOperationClaimBusinessRules.UserOperationClaimShouldExistWhenSelected(userOperationClaim);

            GetByIdUserOperationClaimResponse userOperationClaimDto = _mapper.Map<GetByIdUserOperationClaimResponse>(userOperationClaim);
            return CustomResponseDto<GetByIdUserOperationClaimResponse>.Success((int)HttpStatusCode.OK, userOperationClaimDto, true);
        }
    }
}
