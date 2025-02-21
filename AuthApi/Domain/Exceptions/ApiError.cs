using System.Text.Json;

namespace AuthApi.Domain
{
    public class ApiError(string message, int status) : Exception(message) {
        public int Status { get; set; } = status;

        public string ToJson()
        {
            return JsonSerializer.Serialize(new { message = Message, status = Status });
        }
    }
}