using System.Net;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using Core.Persistence.Paging;
using MediatR;

namespace Application.Features.UserOperationClaims.Queries.GetList;

public class GetListUserOperationClaimQuery : IRequest<CustomResponseDto<GetPagedListResponse<GetListUserOperationClaimListItemDto>>>
{
    public PageRequest PageRequest { get; set; }

    public GetListUserOperationClaimQuery()
    {
        PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
    }

    public GetListUserOperationClaimQuery(PageRequest pageRequest)
    {
        PageRequest = pageRequest;
    }

    public class GetListUserOperationClaimQueryHandler
        : IRequestHandler<GetListUserOperationClaimQuery, CustomResponseDto<GetPagedListResponse<GetListUserOperationClaimListItemDto>>>
    {
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;
        private readonly IMapper _mapper;

        public GetListUserOperationClaimQueryHandler(IUserOperationClaimRepository userOperationClaimRepository, IMapper mapper)
        {
            _userOperationClaimRepository = userOperationClaimRepository;
            _mapper = mapper;
        }

        public async Task<CustomResponseDto<GetPagedListResponse<GetListUserOperationClaimListItemDto>>> Handle(
            GetListUserOperationClaimQuery request,
            CancellationToken cancellationToken)
        {
            IPaginate<UserOperationClaim> userOperationClaims = await _userOperationClaimRepository.GetPagedListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize);

            GetPagedListResponse<GetListUserOperationClaimListItemDto> mappedUserOperationClaimListModel = _mapper.Map<
                GetPagedListResponse<GetListUserOperationClaimListItemDto>
            >(userOperationClaims);

            return CustomResponseDto<GetPagedListResponse<GetListUserOperationClaimListItemDto>>.Success((int)HttpStatusCode.OK, mappedUserOperationClaimListModel, true);
        }
    }
}
