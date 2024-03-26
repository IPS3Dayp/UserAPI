using UserAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace UserAPI.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<User> _userCollection;

        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _userCollection = database.GetCollection<User>(mongoDBSettings.Value.CollectioName);
        }

        public async Task CreateAsync(User user)
        {
            await _userCollection.InsertOneAsync(user);
            return;
        }

        public async Task<List<User>> GetAsync()
        {
            return await _userCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<List<int>> GetActivityListByUserIdAsync(string userId)
        {
            var filter = Builders<User>.Filter.Eq("_id", new ObjectId(userId));

            var result = await _userCollection.Find(filter).FirstOrDefaultAsync();
            if (result == null)
                return null;

            return result.activityIds;
        }

        public async Task AddToActivityAsync(string id, int activity)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq("id", id);
            UpdateDefinition<User> update = Builders<User>.Update.AddToSet<int>("Activities", activity);
            await _userCollection.UpdateOneAsync(filter, update);
            return;
        }
    }
}
