using Backend.DTOs.HomeDTOs;
using Backend.DTOs.SharedDTOs;

namespace Backend.Services.Interfaces
{
    public interface IHomeService
    {
        Task<Result<HomeDTO>> GetHomeData(string userId);
    }
}
