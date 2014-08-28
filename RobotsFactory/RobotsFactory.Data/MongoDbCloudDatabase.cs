namespace RobotsFactory.Data
{
    using System;
    using System.Linq;
    using MongoDB.Driver;

    public class MongoDbCloudDatabase
    {
        private readonly string connectionString;
        private readonly string databaseName;

        public MongoDbCloudDatabase()
            : this(ConnectionStrings.Default.MongoDbCloudDatabase, "robotsfactorydata")
        {
        }

        public MongoDbCloudDatabase(string connectionString, string databaseName)
        {
            this.connectionString = connectionString;
            this.databaseName = databaseName;
        }

        public void PrintCollectionItems(string collectionName)
        {
            Console.WriteLine("Loading data from MongoDB Cloud Database...\n");

            var database = this.GetDatabase(this.databaseName);
            var collection = database.GetCollection(collectionName);

            foreach (var item in collection.FindAll())
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();
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