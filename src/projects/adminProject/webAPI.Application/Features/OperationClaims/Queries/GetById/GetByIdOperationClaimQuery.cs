using AutoMapper;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using webAPI.Application.Features.OperationClaims.Rules;
using webAPI.Application.Services.Repositories;

namespace webAPI.Application.Features.OperationClaims.Queries.GetById
{
    public class GetByIdOperationClaimQuery : IRequest<CustomResponseDto<GetByIdOperationClaimResponse>>
    {
        public Guid Id { get; set; }

        public class GetByIdOperationClaimQueryHandler : IRequestHandler<GetByIdOperationClaimQuery, CustomResponseDto<GetByIdOperationClaimResponse>>
        {
            private readonly IOperationClaimRepository _operationClaimRepository;
            private readonly IMapper _mapper;
            private readonly OperationClaimBusinessRules _operationClaimBusinessRules;

            public GetByIdOperationClaimQueryHandler(
                IOperationClaimRepository operationClaimRepository,
                IMapper mapper,
                OperationClaimBusinessRules operationClaimBusinessRules
            )
            {
                _operationClaimRepository = operationClaimRepository;
                _mapper = mapper;
                _operationClaimBusinessRules = operationClaimBusinessRules;
            }

            public async Task<CustomResponseDto<GetByIdOperationClaimResponse>> Handle(GetByIdOperationClaimQuery request, CancellationToken cancellationToken)
            {
                OperationClaim? operationClaim = await _operationClaimRepository.GetAsync(
                    predicate: b => b.Id == request.Id,
                    include: q => q.Include(oc => oc.UserOperationClaims),
                    cancellationToken: cancellationToken
                );
                await _operationClaimBusinessRules.OperationClaimShouldExistWhenSelected(operationClaim);

                GetByIdOperationClaimResponse response = _mapper.Map<GetByIdOperationClaimResponse>(operationClaim);
                return CustomResponseDto<GetByIdOperationClaimResponse>.Success((int)HttpStatusCode.OK, response, true);
            }
        }
    }

}
