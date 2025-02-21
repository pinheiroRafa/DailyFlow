using ConsolidatedAPI.Domain.Entities;

namespace ConsolidatedAPI.Dtos
{
    public class ReleaseResponse
    {
        public long TotalValue { get; set; }
        public long TotalDebitValue { get; set; }
        public long TotalCreditValue { get; set; }
        public List<Release>? Releases { get; set; }
    }
}