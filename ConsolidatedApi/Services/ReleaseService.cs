

using AuthApi.Interfaces;
using ConsolidatedAPI.Dtos;
using ConsolidatedAPI.Interfaces;

namespace ConsolidatedAPI.Services 
{
    public class ReleaseService: IReleaseService
    {

        private readonly IReleaseRepository _releaseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheService _cacheService;

        public ReleaseService(IReleaseRepository releaseRepository, IHttpContextAccessor httpContextAccessor, ICacheService cacheService)
        {
            _releaseRepository = releaseRepository;
            _httpContextAccessor = httpContextAccessor;
            _cacheService = cacheService;
        }

        public async Task<ReleaseResponse> Consolidated() 
        {
            var user = _httpContextAccessor.HttpContext!.User;
            var companyId = user.FindFirst("companyId")?.Value!;

            var cachedResponse = await _cacheService.Find<ReleaseResponse>(companyId);
            if (cachedResponse != null) {
                return cachedResponse;
            }
            var results = await _releaseRepository.Today(companyId);
            long totalValue = 0;
            long totalDebitValue = 0;
            long totalCreditValue = 0;
            for (var i = 0; i < results.Count; i++)
            {
                var release = results[i];
                totalValue += release.Value;
                if (release.Value < 0) {
                    totalDebitValue += release.Value;
                } else {
                    totalCreditValue += release.Value;
                }
            }
            var response = new ReleaseResponse {
                TotalCreditValue = totalCreditValue,
                TotalDebitValue = totalDebitValue,
                TotalValue = totalValue,
                Releases = results,
            };
            await _cacheService.Create(companyId, 10, response);
            return response;
        }
    }
}