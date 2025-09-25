using System.Net;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using MediatR;
using webAPI.Application.Features.OperationClaims.Rules;
using webAPI.Application.Services.Repositories;
using static webAPI.Application.Features.OperationClaims.Constants.OperationClaimsOperationClaims;

namespace webAPI.Application.Features.OperationClaims.Commands.Create
{
    public class CreateOperationClaimCommand : IRequest<CustomResponseDto<CreatedOperationClaimResponse>>, ISecuredRequest
    {
        public string Name { get; set; }

        public CreateOperationClaimCommand()
        {
            Name = string.Empty;
        }

        public CreateOperationClaimCommand(string name)
        {
            Name = name;
        }

        public string[] Roles => new[] { Admin, Write, Add };

        public class CreateOperationClaimCommandHandler : IRequestHandler<CreateOperationClaimCommand, CustomResponseDto<CreatedOperationClaimResponse>>
        {
            private readonly IOperationClaimRepository _operationClaimRepository;
            private readonly IMapper _mapper;
            private readonly OperationClaimBusinessRules _operationClaimBusinessRules;

            public CreateOperationClaimCommandHandler(
                IOperationClaimRepository operationClaimRepository,
                IMapper mapper,
                OperationClaimBusinessRules operationClaimBusinessRules
            )
            {
                _operationClaimRepository = operationClaimRepository;
                _mapper = mapper;
                _operationClaimBusinessRules = operationClaimBusinessRules;
            }

            public async Task<CustomResponseDto<CreatedOperationClaimResponse>> Handle(CreateOperationClaimCommand request, CancellationToken cancellationToken)
            {
                await _operationClaimBusinessRules.OperationClaimNameShouldNotExistWhenCreating(request.Name);
                OperationClaim mappedOperationClaim = _mapper.Map<OperationClaim>(request);

                OperationClaim createdOperationClaim = await _operationClaimRepository.AddAsync(mappedOperationClaim);

                CreatedOperationClaimResponse response = _mapper.Map<CreatedOperationClaimResponse>(createdOperationClaim);
                return CustomResponseDto<CreatedOperationClaimResponse>.Success((int)HttpStatusCode.OK, response, true);
            }
        }
    }
}
