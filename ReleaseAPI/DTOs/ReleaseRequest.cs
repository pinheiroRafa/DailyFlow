namespace ReleaseAPI.Dtos
{
    public class ReleaseRequest
    {
        public required long Value { get; set; }
        public required string Description { get; set; }
    }
}