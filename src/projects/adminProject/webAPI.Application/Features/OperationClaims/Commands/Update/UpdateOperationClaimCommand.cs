using System.Net;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using MediatR;
using webAPI.Application.Features.OperationClaims.Constants;
using webAPI.Application.Features.OperationClaims.Rules;
using webAPI.Application.Services.Repositories;
using static webAPI.Application.Features.OperationClaims.Constants.OperationClaimsOperationClaims;

namespace webAPI.Application.Features.OperationClaims.Commands.Update
{
    public class UpdateOperationClaimCommand : IRequest<CustomResponseDto<UpdatedOperationClaimResponse>>, ISecuredRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public UpdateOperationClaimCommand()
        {
            Name = string.Empty;
        }

        public UpdateOperationClaimCommand(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public string[] Roles => new[] { Admin, Write, OperationClaimsOperationClaims.Update };

        public class UpdateOperationClaimCommandHandler : IRequestHandler<UpdateOperationClaimCommand, CustomResponseDto<UpdatedOperationClaimResponse>>
        {
            private readonly IOperationClaimRepository _operationClaimRepository;
            private readonly IMapper _mapper;
            private readonly OperationClaimBusinessRules _operationClaimBusinessRules;

            public UpdateOperationClaimCommandHandler(
                IOperationClaimRepository operationClaimRepository,
                IMapper mapper,
                OperationClaimBusinessRules operationClaimBusinessRules)
            {
                _operationClaimRepository = operationClaimRepository;
                _mapper = mapper;
                _operationClaimBusinessRules = operationClaimBusinessRules;
            }

            public async Task<CustomResponseDto<UpdatedOperationClaimResponse>> Handle(UpdateOperationClaimCommand request, CancellationToken cancellationToken)
            {
                OperationClaim? operationClaim = await _operationClaimRepository.GetAsync(
                    predicate: oc => oc.Id == request.Id,
                    cancellationToken: cancellationToken);
                await _operationClaimBusinessRules.OperationClaimShouldExistWhenSelected(operationClaim);
                await _operationClaimBusinessRules.OperationClaimNameShouldNotExistWhenUpdating(request.Id, request.Name);
                OperationClaim mappedOperationClaim = _mapper.Map(request, destination: operationClaim!);

                OperationClaim updatedOperationClaim = await _operationClaimRepository.UpdateAsync(mappedOperationClaim);

                UpdatedOperationClaimResponse response = _mapper.Map<UpdatedOperationClaimResponse>(updatedOperationClaim);
                return CustomResponseDto<UpdatedOperationClaimResponse>.Success((int)HttpStatusCode.OK, response, true);
            }
        }
    }
}
