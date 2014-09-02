namespace RobotsFactory.MongoDb
{
    using System;
    using System.Linq;
    using MongoDB.Driver;
    using RobotsFactory.MongoDb.Contracts;
    using RobotsFactory.MongoDb.Mapping;

    public class MongoDbContext : IMongoDbContext
    {
        private readonly string connectionString;
        private readonly string databaseName;

        public MongoDbContext()
            : this(ConnectionStrings.Default.MongoDbCloudDatabase, ConnectionStrings.Default.MongoDbDefaultDatabase)
        {
        }

        public MongoDbContext(string connectionString, string databaseName)
        {
            this.connectionString = connectionString;
            this.databaseName = databaseName;
        }
       
        public MongoCollection<CountryMap> Countries
        {
            get
            {
                return this.GetCollection<CountryMap>("Countries");
            }
        }

        public MongoCollection<CityMap> Cities
        {
            get
            {
                return this.GetCollection<CityMap>("Cities");
            }
        }

        public MongoCollection<AddressMap> Addresses
        {
            get
            {
                return this.GetCollection<AddressMap>("Addresses");
            }
        }

        public MongoCollection<ProductMap> Products
        {
            get
            {
                return this.GetCollection<ProductMap>("Products");
            }
        }

        public MongoCollection<ProductTypeMap> ProductTypes
        {
            get
            {
                return this.GetCollection<ProductTypeMap>("ProductTypes");
            }
        }

        public MongoCollection<ManufacturerMap> Manufacturers
        {
            get
            {
                return this.GetCollection<ManufacturerMap>("Manufacturers");
            }
        }

        public MongoCollection<ManufacturerExpenseMap> ManufacturerExpenses
        {
            get
            {
                return this.GetCollection<ManufacturerExpenseMap>("ManufacturerExpenses");
            }
        }

        private MongoCollection<T> GetCollection<T>(string collectionName)
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