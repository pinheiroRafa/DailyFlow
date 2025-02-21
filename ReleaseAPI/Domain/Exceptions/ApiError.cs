using System.Text.Json;

namespace ReleaseAPI.Domain.Exceptions
{
    public class ApiError(string message, int status) : Exception(message) {
        public int Status { get; set; } = status;

        public string ToJson()
        {
            return JsonSerializer.Serialize(new { message = Message, status = Status });
        }
    }
}