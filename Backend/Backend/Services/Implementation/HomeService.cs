using AutoMapper;
using Backend.DTOs.AuthDTOs;
using Backend.DTOs.HomeDTOs;
using Backend.DTOs.SharedDTOs;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;

namespace Backend.Services.Implementation
{
    public class HomeService: IHomeService
    {
        private readonly ICompanyRepository _companyRepo;
        private readonly IMapper _mapper;
        public HomeService(ICompanyRepository companyRepo, IMapper mapper)
        {
            _companyRepo = companyRepo;
            _mapper = mapper;
        }

        public async Task<Result<HomeDTO>> GetHomeData(string userId)
        {
            var user = await _companyRepo.FindByIdAsync(userId);
            if (user == null)
                return Result<HomeDTO>.Failure("User not found");

            var result = _mapper.Map<HomeDTO>(user);
            return Result<HomeDTO>.Success(result);
        }
    }
}
