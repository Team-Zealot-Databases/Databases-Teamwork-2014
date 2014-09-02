namespace RobotsFactory.MongoDb.Contracts
{
    using MongoDB.Driver;
    using RobotsFactory.MongoDb.Mapping;

    public interface IMongoDbContext
    {
        MongoCollection<CountryMap> Countries { get; }

        MongoCollection<CityMap> Cities { get; }

        MongoCollection<AddressMap> Addresses { get; }

        MongoCollection<ProductMap> Products { get; }

        MongoCollection<ProductTypeMap> ProductTypes { get; }

        MongoCollection<ManufacturerMap> Manufacturers { get; }

        MongoCollection<ManufacturerExpenseMap> ManufacturerExpenses { get; }
    }
}