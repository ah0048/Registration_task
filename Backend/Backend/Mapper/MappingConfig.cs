using AutoMapper;
using Backend.DTOs.AuthDTOs;
using Backend.DTOs.HomeDTOs;
using Backend.Models;

namespace Backend.Mapper
{
    public class MappingConfig: Profile
    {
        public MappingConfig()
        {
            CreateMap<RegisterDTO, CompanyUser>().ReverseMap();
            CreateMap<RegisterResultDTO, CompanyUser>().ReverseMap();
            CreateMap<HomeDTO, CompanyUser>().ReverseMap();
        }
    }
}
