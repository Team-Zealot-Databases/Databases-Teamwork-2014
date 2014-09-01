namespace RobotsFactory.MongoDb.Contracts
{
    using MongoDB.Driver;

    public interface IMongoDbContext
    {
        MongoCollection<T> GetCollection<T>(string collectionName);
    }
}