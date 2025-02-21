using MongoDB.Driver;
using AuthApi.Domain.Entities;
using AuthApi.Interfaces;

namespace AuthApi.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IMongoCollection<Company> _usersCollection;

        public CompanyRepository(IMongoDatabase database)
        {
            _usersCollection = database.GetCollection<Company>("companies");
        }

        public async Task<Company?> GetByOwnerId(string ownerId)
        {
            return await _usersCollection.Find(user => user.UserId == ownerId).FirstOrDefaultAsync();
        }
    }
}
