namespace RobotsFactory.Data.MongoDb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using RobotsFactory.Data;

    public class MongoDbCloudDatabase
    {
        private readonly string connectionString;
        private readonly string databaseName;

        public MongoDbCloudDatabase()
            : this(ConnectionStrings.Default.MongoDbCloudDatabase, ConnectionStrings.Default.MongoDbDefaultDatabase)
        {
        }

        public MongoDbCloudDatabase(string connectionString, string databaseName)
        {
            this.connectionString = connectionString;
            this.databaseName = databaseName;
        }
       
        public IEnumerable<BsonDocument> GetItemsFromCollection(string collectionName)
        {
            var database = this.GetDatabase(this.databaseName);
            var collection = database.GetCollection(collectionName);

            return collection.FindAll();
        }

        private MongoDatabase GetDatabase(string databaseName)
        {
            var mongoClient = new MongoClient(this.connectionString);
            var mongoServer = mongoClient.GetServer();
            
            var database = mongoServer.GetDatabase(databaseName);
            return database;
        }
    }
}