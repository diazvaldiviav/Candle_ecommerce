using Candle_API.Data.Entities;
using Candle_API.Data.DTOs.Auth;

namespace Candle_API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> Login(LoginDTO loginDto);
        Task<string> GenerateToken(User user, IList<string> roles);
    }
}
