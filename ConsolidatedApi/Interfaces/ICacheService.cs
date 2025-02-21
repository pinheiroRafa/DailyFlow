using ConsolidatedAPI.Dtos;

namespace AuthApi.Interfaces
{
    public interface ICacheService
    {
        Task<bool> Create<T>(string key, int minutes, T value);
        Task<T?> Find<T>(string key);
    }
}