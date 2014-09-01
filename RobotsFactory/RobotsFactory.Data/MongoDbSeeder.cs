namespace RobotsFactory.Data
{
    using System;
    using System.Linq;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.Models;
    using RobotsFactory.MongoDb;
    using RobotsFactory.MongoDb.Contracts;
    using RobotsFactory.MongoDb.Mapping;

    public class MongoDbSeeder
    {
        private readonly IMongoDbContext mongoDb;
        private readonly IRobotsFactoryData robotsFactoryData;

        public MongoDbSeeder(IRobotsFactoryData robotsFactoryData)
            : this(robotsFactoryData, new MongoDbContext(ConnectionStrings.Default.MongoDbCloudDatabase, ConnectionStrings.Default.MongoDbDefaultDatabase))
        {
        }

        public MongoDbSeeder(IRobotsFactoryData robotsFactoryData, IMongoDbContext mongoDb)
        {
            this.robotsFactoryData = robotsFactoryData;
            this.mongoDb = mongoDb;
        }

        public void Seed()
        {
            this.AddCountries();
            this.AddCities();
            this.AddAddresses();
            this.AddManufacturers();
            this.AddProductTypes();
            this.AddProducts();
            this.SaveChanges();
        }

        public void AddCountries()
        {
            if (this.robotsFactoryData.Countries.All().Any())
            {
                return;
            }

            var countries = this.mongoDb.GetCollection<CountryMap>("Countries").FindAll();
            
            foreach (var country in countries)
            {
                this.robotsFactoryData.Countries.Add(new Country()
                {
                    CountryId = country.CountryId,
                    Name = country.Name
                });
            }
        }

        public void AddCities()
        {
            if (this.robotsFactoryData.Cities.All().Any())
            {
                return;
            }

            var cities = this.mongoDb.GetCollection<CityMap>("Cities").FindAll();

            foreach (var city in cities)
            {
                this.robotsFactoryData.Cities.Add(new City()
                {
                    CityId = city.CityId,
                    CountryId = city.CountryId,
                    Name = city.Name
                });
            }
        }

        public void AddAddresses()
        {
            if (this.robotsFactoryData.Addresses.All().Any())
            {
                return;
            }

            var addresses = this.mongoDb.GetCollection<AddressMap>("Addresses").FindAll();

            foreach (var address in addresses)
            {
                this.robotsFactoryData.Addresses.Add(new Address()
                {
                    AddressId = address.AddressId,
                    AddressText = address.AddressText,
                    CityId = address.CityId,
                });
            }
        }

        public void AddManufacturers()
        {
            if (this.robotsFactoryData.Manufacturers.All().Any())
            {
                return;
            }

            var manufacturers = this.mongoDb.GetCollection<ManufacturerMap>("Manufacturers").FindAll();

            foreach (var manufacturer in manufacturers)
            {
                this.robotsFactoryData.Manufacturers.Add(new Manufacturer()
                {
                    ManufacturerId = manufacturer.ManufacturerId,
                    Name = manufacturer.Name,
                    AddressId = manufacturer.AddressId
                });
            }
        }

        public void AddProductTypes()
        {
            if (this.robotsFactoryData.ProductTypes.All().Any())
            {
                return;
            }

            var productTypes = this.mongoDb.GetCollection<ProductTypeMap>("ProductTypes").FindAll();

            foreach (var productType in productTypes)
            {
                this.robotsFactoryData.ProductTypes.Add(new ProductType()
                {
                    ProductTypeId = productType.ProductTypeId,
                    Name = productType.Name
                });
            }
        }

        public void AddProducts()
        {
            if (this.robotsFactoryData.Products.All().Any())
            {
                return;
            }

            var products = this.mongoDb.GetCollection<ProductMap>("Products").FindAll();

            foreach (var product in products)
            {
                this.robotsFactoryData.Products.Add(new Product()
                {
                    ProductId = product.ProductId,
                    Name = product.ProductName,
                    Dimensions = product.Dimensions,
                    Weight = (decimal)product.Weight,
                    ProductTypeId = product.ProductTypeId,
                    Description = product.ShortDescription,
                    ManufacturerId = product.ManufacturerId,
                    ReleaseYear = product.ReleaseYear,
                    Price = (decimal)product.Price
                });
            }
        }

        public void SaveChanges()
        {
            this.robotsFactoryData.SaveChanges();
        }
    }
}