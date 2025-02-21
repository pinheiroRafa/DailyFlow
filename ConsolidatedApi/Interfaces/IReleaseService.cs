using ConsolidatedAPI.Dtos;

namespace AuthApi.Interfaces
{
    public interface IReleaseService
    {
        Task<ReleaseResponse> Consolidated();
    }
}