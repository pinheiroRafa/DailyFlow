using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReleaseApi.Domain.Entities
{
    public class Release
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("companyId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CompanyId { get; set; } = string.Empty;
        [BsonElement("value")]
        [BsonRepresentation(BsonType.Int64)]
        public required long Value { get; set; }

        [BsonElement("createdAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public required DateTime CreatedAt { get; set; }
    }
}