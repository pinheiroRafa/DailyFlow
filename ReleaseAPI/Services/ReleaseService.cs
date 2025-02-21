

using AuthApi.Interfaces;
using ReleaseAPI.Dtos;

namespace ReleaseAPI.Services 
{
    public class ReleaseService: IReleaseService
    {

        private readonly IQueueService _queueService;
         private readonly IHttpContextAccessor _httpContextAccessor;

        public ReleaseService(IQueueService queueService, IHttpContextAccessor httpContextAccessor)
        {
            _queueService = queueService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ReleaseResponse> Create(ReleaseRequest param) 
        {
            var user = _httpContextAccessor.HttpContext!.User;
            var userId = user.FindFirst("userId")?.Value!;
            var companyId = user.FindFirst("companyId")?.Value!;

            await _queueService.SendMessage(new ReleaseApi.Domain.Entities.Release {
                Description = param.Description,
                Value = param.Value,
                UserId = userId,
                CompanyId = companyId,
                CreatedAt = DateTime.Now
            });
            
            return  new ReleaseResponse { OK = true };
        }
    }
}