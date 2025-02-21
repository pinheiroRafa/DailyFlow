using AuthApi.Domain.Entities;

namespace AuthApi.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmail(string email);
        Task CreateUser(User user);
    }
}