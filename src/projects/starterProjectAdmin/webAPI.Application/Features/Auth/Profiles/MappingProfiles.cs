using AutoMapper;
using Core.Domain.Entities;
using webAPI.Application.Features.Auth.Commands.RevokeToken;

namespace webAPI.Application.Features.Auth.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<RefreshToken, RevokedTokenResponse>().ReverseMap();
        }
    }
}
