using Backend.Models;
using Backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Backend.Repositories.Implementations
{
    public class CompanyRepository: ICompanyRepository
    {
        public UserManager<CompanyUser> _userManager { get; }
        public CompanyRepository(UserManager<CompanyUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<CompanyUser> FindByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

        public Task<CompanyUser> FindByIdAsync(string Id)
        {
            return _userManager.FindByIdAsync(Id);
        }

        public Task<IdentityResult> CreateAsync(CompanyUser user)
        {
            return _userManager.CreateAsync(user);
        }

        public Task<IdentityResult> CreateAsync(CompanyUser user, string password)
        {
            return _userManager.CreateAsync(user, password);
        }

        public Task<IdentityResult> AddPasswordAsync(CompanyUser user, string password)
        {
            return _userManager.AddPasswordAsync(user, password);
        }

    }
}
