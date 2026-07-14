using CelebrationPassports.Web.Models.Account;

namespace CelebrationPassports.Web.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthResult> RegisterAsync(RegisterViewModel model);
        Task<AuthResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync(Guid sessionId);
    }
}
