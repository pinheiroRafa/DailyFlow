
using MongoDB.Driver;
using ConsolidatedAPI.Domain.Entities;
using ConsolidatedAPI.Interfaces;
using MongoDB.Bson;

namespace ConsolidatedAPI.Repositories
{
    public class ReleaseRepository(IMongoDatabase database) : IReleaseRepository
    {
        private readonly IMongoCollection<Release> _releasesCollection = database.GetCollection<Release>("releases");

        public async Task<List<Release>> Today(string companyId)
        {
            var today = DateTime.UtcNow.Date;  
            var tomorrow = today.AddDays(1);

            var filter = Builders<Release>.Filter.Gte(r => r.CreatedAt, today) 
                & Builders<Release>.Filter.Lt(r => r.CreatedAt, tomorrow)
                & Builders<Release>.Filter.Eq(r => r.CompanyId, companyId);
            var results = await _releasesCollection.FindAsync(filter);
            return results.ToList();
        }
    }
}
