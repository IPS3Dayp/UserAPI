using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace UserAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string email { get; set; }
        [BsonElement("Activities")]
        public List<int> activityIds { get; set; } = null!;
    }
}
