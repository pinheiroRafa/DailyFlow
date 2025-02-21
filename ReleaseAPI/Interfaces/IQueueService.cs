using ReleaseAPI.Dtos;

namespace AuthApi.Interfaces
{
    public interface IQueueService
    {
        Task<string> SendMessage<T>(T param);
        Task ListenForMessagesAsync();
    }
}