using AuthApi.Domain.Entities;

namespace AuthApi.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByOwnerId(string ownerId);
    }
}