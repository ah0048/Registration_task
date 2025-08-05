using AutoMapper;
using Backend.DTOs;
using Backend.Models;

namespace Backend.Mapper
{
    public class MappingConfig: Profile
    {
        public MappingConfig()
        {
            CreateMap<RegisterDTO, CompanyUser>().ReverseMap();
            CreateMap<RegisterResultDTO, CompanyUser>().ReverseMap();
        }
    }
}
