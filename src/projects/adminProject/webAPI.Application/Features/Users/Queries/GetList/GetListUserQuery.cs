using System.Net;
using AutoMapper;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using Core.Persistence.Paging;
using MediatR;
using webAPI.Application.Services.Repositories;

namespace webAPI.Application.Features.Users.Queries.GetList
{
    public class GetListUserQuery : IRequest<CustomResponseDto<GetPagedListResponse<GetListUserListItemDto>>>
    {
        public PageRequest PageRequest { get; set; }

        public GetListUserQuery()
        {
            PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
        }

        public GetListUserQuery(PageRequest pageRequest)
        {
            PageRequest = pageRequest;
        }

        public class GetListUserQueryHandler : IRequestHandler<GetListUserQuery, CustomResponseDto<GetPagedListResponse<GetListUserListItemDto>>>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;

            public GetListUserQueryHandler(IUserRepository userRepository, IMapper mapper)
            {
                _userRepository = userRepository;
                _mapper = mapper;
            }

            public async Task<CustomResponseDto<GetPagedListResponse<GetListUserListItemDto>>> Handle(GetListUserQuery request, CancellationToken cancellationToken)
            {
                IPaginate<User> users = await _userRepository.GetPagedListAsync(
                    index: request.PageRequest.PageIndex,
                    size: request.PageRequest.PageSize,
                    cancellationToken: cancellationToken);

                GetPagedListResponse<GetListUserListItemDto> response = _mapper.Map<GetPagedListResponse<GetListUserListItemDto>>(users);
                return CustomResponseDto<GetPagedListResponse<GetListUserListItemDto>>.Success((int)HttpStatusCode.OK, response, true);
            }
        }
    }
}
