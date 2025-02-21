using AuthApi.Dtos;

namespace AuthApi.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(string username, string password);
    }
}