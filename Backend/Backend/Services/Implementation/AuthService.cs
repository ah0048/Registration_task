using AutoMapper;
using Backend.DTOs.AuthDTOs;
using Backend.Helpers;
using Backend.Models;
using Backend.Repositories.Interfaces;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Services.Implementation
{
    public class AuthService: IAuthService
    {
        private readonly IPhotoService _photoservice;
        private readonly IEmailSender _emailsender;
        private readonly ICompanyRepository _companyRepo;
        private readonly IMapper _mapper;
        private readonly JwtSettings jwtSettings;

        public AuthService(IPhotoService photoservice, IEmailSender emailsender, ICompanyRepository companyRepo, IOptions<JwtSettings> options, IMapper mapper)
        {
            _photoservice = photoservice;
            _emailsender = emailsender;
            _companyRepo = companyRepo;
            jwtSettings = options.Value;
            _mapper = mapper;
        }

        private string GenerateOtp()
        {
            var otp =  RandomNumberGenerator.GetInt32(100_000, 1_000_000);
            return otp.ToString();
        }

        private string GenerateToken(CompanyUser user)
        {
            var userData = new List<Claim>();
            userData.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            userData.Add(new Claim(ClaimTypes.Email, user.Email));

            #region SigningCredentials
            var key = jwtSettings.Key;
            var secreteKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            var signingCredentials = new SigningCredentials(secreteKey, SecurityAlgorithms.HmacSha256);
            #endregion

            JwtSecurityToken tokenObject = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: userData,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: signingCredentials
                );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return token;
        }

        private async Task SendOtpMail(string Otp, string companyEmail, string companyName)
        {
            string emailBody = "<div style='width:100%; background-color:grey'>";
            emailBody += $"<h1> Hi {companyName}, Thanks for registering</h1>";
            emailBody += "<h2>Please enter the OTP and complete the registeration</h2>";
            emailBody += $"<h2>your OTP is: {Otp}</h2>";
            emailBody += "</div>";

            string emailSubject = "Thanks for registeration: OTP";

            await _emailsender.SendEmailAsync(companyEmail, emailSubject, emailBody);
        }

        public async Task<Result<RegisterResultDTO>> Register(RegisterDTO registerDTO)
        {
            try
            {
                
                var existingUser = await _companyRepo.FindByEmailAsync(registerDTO.Email);
                if (existingUser != null)
                    return Result<RegisterResultDTO>.Failure("Email address is already registered");

                var user = _mapper.Map<CompanyUser>(registerDTO);

                
                if (registerDTO.Logo != null)
                {
                    try
                    {
                        var result = await _photoservice.AddPhotoAsync(registerDTO.Logo);
                        if (result?.SecureUrl == null)
                            return Result<RegisterResultDTO>.Failure("Failed to upload company logo");
                        
                        user.LogoUrl = result.SecureUrl.AbsoluteUri;
                        user.LogoPublicId = result.PublicId;
                    }
                    catch (Exception ex)
                    {
                        return Result<RegisterResultDTO>.Failure($"Logo upload failed: {ex.Message}");
                    }
                }

                
                string otp = GenerateOtp();
                user.OtpCode = otp;
                user.OtpExpiry = DateTime.Now.AddMinutes(10);

                
                var createResult = await _companyRepo.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var errors = createResult.Errors.Select(e => e.Description).ToList();
                    return Result<RegisterResultDTO>.Failure(errors);
                }

                
                try
                {
                    await SendOtpMail(otp, user.Email, user.CompanyNameEn);
                }
                catch (Exception ex)
                {
                    
                    var registerResult = _mapper.Map<RegisterResultDTO>(user);
                    return Result<RegisterResultDTO>.Failure($"User registered successfully but email sending failed: {ex.Message}");
                }

                var successResult = _mapper.Map<RegisterResultDTO>(user);
                return Result<RegisterResultDTO>.Success(successResult);
            }
            catch (Exception ex)
            {
                return Result<RegisterResultDTO>.Failure($"An unexpected error occurred during registration: {ex.Message}");
            }
        }

        public async Task<Result<bool>> IsOtpValid(CheckOtpDTO checkOtp)
        {
            try
            {
                var user = await _companyRepo.FindByIdAsync(checkOtp.Id);
                if (user == null)
                    return Result<bool>.Failure("User not found");

                if (string.IsNullOrWhiteSpace(user.OtpCode))
                    return Result<bool>.Failure("No OTP found for this user");

                if (user.OtpExpiry == null || user.OtpExpiry <= DateTime.Now)
                    return Result<bool>.Success(false);

                bool isValid = user.OtpCode == checkOtp.OtpCode;
                return Result<bool>.Success(isValid);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"An unexpected error occurred during OTP validation: {ex.Message}");
            }
        }

        public async Task<Result> SetPassword(SetPasswordDTO setPassword)
        {
            try
            {
                var user = await _companyRepo.FindByIdAsync(setPassword.Id);
                if (user == null)
                    return Result.Failure("User not found");

                
                if (string.IsNullOrWhiteSpace(user.OtpCode) || user.OtpExpiry == null || user.OtpExpiry <= DateTime.Now)
                    return Result.Failure("OTP has expired. Please request a new OTP");

                var result = await _companyRepo.AddPasswordAsync(user, setPassword.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Result.Failure(errors);
                }


                user.OtpCode = null;
                user.OtpExpiry = null;
                

                var updateResult = await _companyRepo.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = updateResult.Errors.Select(e => e.Description).ToList();
                    return Result.Failure($"Password set successfully but failed to clear OTP: {string.Join("; ", errors)}");
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"An unexpected error occurred while setting password: {ex.Message}");
            }
        }

        public async Task<Result<string>> Login(LoginDTO loginDTO)
        {
            try
            {
                var user = await _companyRepo.FindByEmailAsync(loginDTO.Email);
                if (user == null)
                    return Result<string>.Failure("Invalid email or password");

                var result = await _companyRepo.CheckPasswordAsync(user, loginDTO.Password);
                if (!result)
                    return Result<string>.Failure("Invalid email or password");

                var token = GenerateToken(user);
                return Result<string>.Success(token);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"An unexpected error occurred during login: {ex.Message}");
            }
        }

        public async Task<Result<RegisterResultDTO>> ResendOtp(string userId)
        {
            try
            {
                var user = await _companyRepo.FindByIdAsync(userId);
                if (user == null)
                    return Result<RegisterResultDTO>.Failure("User not found");

                if (string.IsNullOrWhiteSpace(user.OtpCode))
                    return Result<RegisterResultDTO>.Failure("No OTP found for this user. Please register again");

                if (user.OtpExpiry != null && user.OtpExpiry > DateTime.Now)
                    return Result<RegisterResultDTO>.Failure("Current OTP is still valid. Please use the existing OTP or wait for it to expire");

                string newOtp = GenerateOtp();
                user.OtpCode = newOtp;
                user.OtpExpiry = DateTime.Now.AddMinutes(10);

                var updateResult = await _companyRepo.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = updateResult.Errors.Select(e => e.Description).ToList();
                    return Result<RegisterResultDTO>.Failure(errors);
                }

                try
                {
                    await SendOtpMail(newOtp, user.Email, user.CompanyNameEn);
                }
                catch (Exception ex)
                {
                    return Result<RegisterResultDTO>.Failure($"Failed to send new OTP email: {ex.Message}");
                }

                var result = _mapper.Map<RegisterResultDTO>(user);
                return Result<RegisterResultDTO>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<RegisterResultDTO>.Failure($"An unexpected error occurred while resending OTP: {ex.Message}");
            }
        }
    }
}
