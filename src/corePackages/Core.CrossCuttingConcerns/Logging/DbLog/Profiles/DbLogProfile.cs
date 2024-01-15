using AutoMapper;
using Core.CrossCuttingConcerns.Logging.DbLog.Dto;
using Core.Domain.Entities;

namespace Core.CrossCuttingConcerns.Logging.DbLog.Profiles
{
    public class DbLogProfile : Profile
    {
        public DbLogProfile()
        {
            CreateMap<Log, LogDto>().ReverseMap();
        }
    }
}
