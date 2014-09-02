namespace RobotsFactory.Data
{
    using System;
    using System.Linq;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.Models;
    using RobotsFactory.MongoDb.Contracts;

    public class MongoDbSeeder
    {
        private readonly IMongoDbContext mongoDb;
        private readonly IRobotsFactoryData robotsFactoryData;

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
            
            foreach (var country in this.mongoDb.Countries.FindAll())
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

            foreach (var city in this.mongoDb.Cities.FindAll())
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

            foreach (var address in this.mongoDb.Addresses.FindAll())
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

            foreach (var manufacturer in this.mongoDb.Manufacturers.FindAll())
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

            foreach (var productType in this.mongoDb.ProductTypes.FindAll())
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

            foreach (var product in this.mongoDb.Products.FindAll())
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