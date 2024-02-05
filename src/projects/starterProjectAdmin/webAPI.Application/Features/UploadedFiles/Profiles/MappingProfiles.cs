using AutoMapper;
using Core.Domain.Entities;
using webAPI.Application.Features.UploadedFiles.Dtos;

namespace webAPI.Application.Features.UploadedFiles.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<UploadedFile, UploadedFileDto>().ReverseMap();
        }
    }
}
