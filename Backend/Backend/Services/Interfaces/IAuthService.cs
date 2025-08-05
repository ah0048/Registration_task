using Backend.DTOs.AuthDTOs;
using Backend.Helpers;
using Backend.Models;

namespace Backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result<RegisterResultDTO>> Register(RegisterDTO registerDTO);
        Task<Result<bool>> IsOtpValid(CheckOtpDTO checkOtp);
        Task<Result<string>> SetPassword(SetPasswordDTO setPassword);
        Task<Result<string>> Login(LoginDTO loginDTO);
        Task<Result<RegisterResultDTO>> ResendOtp(string userId);
    }
}
