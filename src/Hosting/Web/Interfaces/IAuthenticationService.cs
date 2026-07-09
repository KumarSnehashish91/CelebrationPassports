using CelebrationPassports.Web.Models.Account;

namespace CelebrationPassports.Web.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> RegisterAsync(RegisterViewModel model);
    }
}