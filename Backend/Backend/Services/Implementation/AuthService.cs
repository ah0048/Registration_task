using AutoMapper;
using Backend.DTOs;
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

        public async Task<RegisterResultDTO> Register(RegisterDTO registerDTO)
        {
            var user = _mapper.Map<CompanyUser>(registerDTO);

            if (registerDTO.Logo != null)
            {
                var result = await _photoservice.AddPhotoAsync(registerDTO.Logo);
                user.LogoUrl = result.SecureUrl.AbsoluteUri;
                user.LogoPublicId = result.PublicId;
            }

            string otp = GenerateOtp();
            user.OtpCode = otp;
            user.OtpExpiry = DateTime.Now.AddMinutes(10);

            var createResult = await _companyRepo.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                throw new ApplicationException(
                    string.Join("; ", createResult.Errors.Select(e => e.Description))
                );
            }

            await SendOtpMail(otp, user.Email, user.CompanyNameEn);

            var registerResult = _mapper.Map<RegisterResultDTO>(user);
            return registerResult;
        }

        public async Task<bool> IsOtpValid(CheckOtpDTO checkOtp)
        {
            var user = await _companyRepo.FindByIdAsync(checkOtp.Id);
            if (user.OtpCode == checkOtp.OtpCode && user.OtpExpiry > DateTime.Now)
            {
                return true;
            }
            return false;
        }

        public async Task SetPassword(SetPasswordDTO setPassword)
        {
            var user = await _companyRepo.FindByIdAsync(setPassword.Id);
            await _companyRepo.AddPasswordAsync(user, setPassword.NewPassword);
        }

        public async Task<string> Login(LoginDTO loginDTO)
        {
            var user = await _companyRepo.FindByEmailAsync(loginDTO.Email);


        }
    }
}
