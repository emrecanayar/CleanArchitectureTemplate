using System.Net;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using Core.Persistence.Paging;
using MediatR;

namespace Application.Features.OperationClaims.Queries.GetList;

public class GetListOperationClaimQuery : IRequest<CustomResponseDto<GetPagedListResponse<GetListOperationClaimListItemDto>>>
{
    public PageRequest PageRequest { get; set; }

    public GetListOperationClaimQuery()
    {
        PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
    }

    public GetListOperationClaimQuery(PageRequest pageRequest)
    {
        PageRequest = pageRequest;
    }

    public class GetListOperationClaimQueryHandler
        : IRequestHandler<GetListOperationClaimQuery, CustomResponseDto<GetPagedListResponse<GetListOperationClaimListItemDto>>>
    {
        private readonly IOperationClaimRepository _operationClaimRepository;
        private readonly IMapper _mapper;

        public GetListOperationClaimQueryHandler(IOperationClaimRepository operationClaimRepository, IMapper mapper)
        {
            _operationClaimRepository = operationClaimRepository;
            _mapper = mapper;
        }

        public async Task<CustomResponseDto<GetPagedListResponse<GetListOperationClaimListItemDto>>> Handle(
            GetListOperationClaimQuery request,
            CancellationToken cancellationToken
        )
        {
            IPaginate<OperationClaim> operationClaims = await _operationClaimRepository.GetPagedListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetPagedListResponse<GetListOperationClaimListItemDto> response = _mapper.Map<GetPagedListResponse<GetListOperationClaimListItemDto>>(
                operationClaims
            );
            return CustomResponseDto<GetPagedListResponse<GetListOperationClaimListItemDto>>.Success((int)HttpStatusCode.OK, response, true);
        }
    }
}
