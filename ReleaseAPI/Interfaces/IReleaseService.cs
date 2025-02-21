using ReleaseAPI.Dtos;

namespace AuthApi.Interfaces
{
    public interface IReleaseService
    {
        Task<ReleaseResponse> Create(ReleaseRequest param);
    }
}