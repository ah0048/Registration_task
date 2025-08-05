using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        Task<CompanyUser> FindByEmailAsync(string email);
        Task<CompanyUser> FindByIdAsync(string Id);
        Task<IdentityResult> CreateAsync(CompanyUser user);
        Task<IdentityResult> CreateAsync(CompanyUser user, string password);
        Task<IdentityResult> AddPasswordAsync(CompanyUser user, string password);
        Task<bool> CheckPasswordAsync(CompanyUser user, string password);
        Task<IdentityResult> UpdateAsync(CompanyUser user);
    }
}
