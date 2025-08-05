using Backend.DTOs.HomeDTOs;
using Backend.Helpers;

namespace Backend.Services.Interfaces
{
    public interface IHomeService
    {
        Task<Result<HomeDTO>> GetHomeData(string userId);
    }
}
