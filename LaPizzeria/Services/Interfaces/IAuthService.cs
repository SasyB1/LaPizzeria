using LaPizzeria.Models.DTO;
using LaPizzeria.Models;

namespace LaPizzeria.Services.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDTO user);
        Task<User> LoginAsync(LoginDTO model);
        Task LogoutAsync();
        int GetCurrentUserId();
        Task<User> GetCurrentUser();
    }
}
