using AutoMapper;
using PCentral.API.DTOs;
using PCentral.API.Models;

namespace PCentral.API.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<StaticPart, StaticPartDto>().ReverseMap();
            CreateMap<Build, BuildDto>().ReverseMap();
        }
    }
}