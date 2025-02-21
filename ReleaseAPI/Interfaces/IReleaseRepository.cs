

using ReleaseApi.Domain.Entities;

namespace ReleaseAPI.Interfaces
{
    public interface IReleaseRepository
    {
        Task<Release> CreateRelease(Release release);
    }
}