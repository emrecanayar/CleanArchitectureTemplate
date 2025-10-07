using AutoMapper;
using Core.Application.Responses;
using Core.Domain.Entities;
using Core.Persistence.Paging;
using webAPI.Application.Features.OperationClaims.Commands.Create;
using webAPI.Application.Features.OperationClaims.Commands.Delete;
using webAPI.Application.Features.OperationClaims.Commands.Update;
using webAPI.Application.Features.OperationClaims.Queries.GetById;
using webAPI.Application.Features.OperationClaims.Queries.GetList;

namespace webAPI.Application.Features.OperationClaims.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<OperationClaim, CreateOperationClaimCommand>().ReverseMap();
            CreateMap<OperationClaim, CreatedOperationClaimResponse>().ReverseMap();
            CreateMap<OperationClaim, UpdateOperationClaimCommand>().ReverseMap();
            CreateMap<OperationClaim, UpdatedOperationClaimResponse>().ReverseMap();
            CreateMap<OperationClaim, DeleteOperationClaimCommand>().ReverseMap();
            CreateMap<OperationClaim, DeletedOperationClaimResponse>().ReverseMap();
            CreateMap<OperationClaim, GetByIdOperationClaimResponse>().ReverseMap();
            CreateMap<OperationClaim, GetListOperationClaimListItemDto>().ReverseMap();
            CreateMap<IPaginate<OperationClaim>, GetPagedListResponse<GetListOperationClaimListItemDto>>().ReverseMap();
        }
    }
}
