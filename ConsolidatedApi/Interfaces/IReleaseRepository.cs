

using ConsolidatedAPI.Domain.Entities;

namespace ConsolidatedAPI.Interfaces
{
    public interface IReleaseRepository
    {
        Task<List<Release>> Today(string companyId);
    }
}