using AutoMapper;
using Core.Application.Responses;
using Core.Domain.Entities;
using Core.Persistence.Paging;
using webAPI.Application.Features.Users.Commands.Create;
using webAPI.Application.Features.Users.Commands.Delete;
using webAPI.Application.Features.Users.Commands.Update;
using webAPI.Application.Features.Users.Commands.UpdateFromAuth;
using webAPI.Application.Features.Users.Queries.GetById;
using webAPI.Application.Features.Users.Queries.GetList;

namespace webAPI.Application.Features.Users.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, CreateUserCommand>().ReverseMap();
            CreateMap<User, CreatedUserResponse>().ReverseMap();
            CreateMap<User, UpdateUserCommand>().ReverseMap();
            CreateMap<User, UpdatedUserResponse>().ReverseMap();
            CreateMap<User, UpdateUserFromAuthCommand>().ReverseMap();
            CreateMap<User, UpdatedUserFromAuthResponse>().ReverseMap();
            CreateMap<User, DeleteUserCommand>().ReverseMap();
            CreateMap<User, DeletedUserResponse>().ReverseMap();
            CreateMap<User, GetByIdUserResponse>().ReverseMap();
            CreateMap<User, GetListUserListItemDto>().ReverseMap();
            CreateMap<IPaginate<User>, GetPagedListResponse<GetListUserListItemDto>>().ReverseMap();
        }
    }
}
