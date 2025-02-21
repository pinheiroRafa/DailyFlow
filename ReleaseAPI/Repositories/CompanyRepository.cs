
using MongoDB.Driver;
using ReleaseApi.Domain.Entities;
using ReleaseAPI.Interfaces;

namespace ReleaseApi.Repositories
{
    public class ReleaseRepository(IMongoDatabase database) : IReleaseRepository
    {
        private readonly IMongoCollection<Release> _releasesCollection = database.GetCollection<Release>("releases");

        public async Task<Release> CreateRelease(Release release)
        {
            await _releasesCollection.InsertOneAsync(release);
            return release;
        }
    }
}
