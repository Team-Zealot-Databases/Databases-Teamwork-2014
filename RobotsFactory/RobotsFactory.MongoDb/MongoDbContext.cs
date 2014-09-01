namespace RobotsFactory.MongoDb
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using MongoDB.Driver;
    using RobotsFactory.MongoDb.Contracts;

    public class MongoDbContext : DbContext, IMongoDbContext
    {
        private readonly string connectionString;
        private readonly string databaseName;

        public MongoDbContext(string connectionString, string databaseName)
        {
            this.connectionString = connectionString;
            this.databaseName = databaseName;
        }
       
        public MongoCollection<T> GetCollection<T>(string collectionName)
        {
            var database = this.GetDatabase();
            var collection = database.GetCollection<T>(collectionName);
            return collection;
        }

        private MongoDatabase GetDatabase()
        {
            var mongoClient = new MongoClient(this.connectionString);
            var mongoServer = mongoClient.GetServer();
            
            var database = mongoServer.GetDatabase(this.databaseName);
            return database;
        }
    }
}