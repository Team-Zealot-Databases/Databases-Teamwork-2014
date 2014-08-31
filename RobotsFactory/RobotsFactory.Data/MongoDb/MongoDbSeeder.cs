namespace RobotsFactory.Data.MongoDb
{
    using System;
    using System.Linq;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.Models;

    public class MongoDbSeeder : CentralDatabaseSeeder
    {
        private readonly MongoDbCloudDatabase mongoDb;
        private readonly IRobotsFactoryData robotsFactoryData;

        public MongoDbSeeder(IRobotsFactoryData robotsFactoryData)
            : this(robotsFactoryData, new MongoDbCloudDatabase())
        {
        }

        public MongoDbSeeder(IRobotsFactoryData robotsFactoryData, MongoDbCloudDatabase mongoDb)
        {
            this.robotsFactoryData = robotsFactoryData;
            this.mongoDb = mongoDb;
        }

        public override void AddCountries()
        {
            if (this.robotsFactoryData.Countries.All().Any())
            {
                return;
            }

            var countries = this.mongoDb.GetItemsFromCollection("Countries");

            foreach (var country in countries)
            {
                this.robotsFactoryData.Countries.Add(new Country()
                {
                    CountryId = country["CountryId"].ToInt32(),
                    Name = country["Name"].ToString()
                });
            }
        }

        public override void AddCities()
        {
            if (this.robotsFactoryData.Cities.All().Any())
            {
                return;
            }

            var cities = this.mongoDb.GetItemsFromCollection("Cities");

            foreach (var city in cities)
            {
                this.robotsFactoryData.Cities.Add(new City()
                {
                    CityId = city["CityId"].ToInt32(),
                    CountryId = city["CountryId"].ToInt32(),
                    Name = city["Name"].ToString()
                });
            }
        }

        public override void AddAddresses()
        {
            if (this.robotsFactoryData.Addresses.All().Any())
            {
                return;
            }

            var addresses = this.mongoDb.GetItemsFromCollection("Addresses");

            foreach (var address in addresses)
            {
                this.robotsFactoryData.Addresses.Add(new Address()
                {
                    AddressId = address["AddressId"].ToInt32(),
                    AddressText = address["AddressText"].ToString(),
                    CityId = address["CityId"].ToInt32(),
                });
            }
        }

        public override void AddManufacturers()
        {
            if (this.robotsFactoryData.Manufacturers.All().Any())
            {
                return;
            }

            var manufacturers = this.mongoDb.GetItemsFromCollection("Manufacturers");

            foreach (var manufacturer in manufacturers)
            {
                this.robotsFactoryData.Manufacturers.Add(new Manufacturer()
                {
                    ManufacturerId = manufacturer["ManufacturerId"].ToInt32(),
                    Name = manufacturer["Name"].ToString(),
                    AddressId = manufacturer["AddressId"].ToInt32(),
                });
            }
        }

        public override void AddProductTypes()
        {
            if (this.robotsFactoryData.ProductTypes.All().Any())
            {
                return;
            }

            var productTypes = this.mongoDb.GetItemsFromCollection("ProductTypes");

            foreach (var productType in productTypes)
            {
                this.robotsFactoryData.ProductTypes.Add(new ProductType()
                {
                    ProductTypeId = productType["ProductTypeId"].ToInt32(),
                    Name = productType["Name"].ToString()
                });
            }
        }

        public override void AddProducts()
        {
            if (this.robotsFactoryData.Products.All().Any())
            {
                return;
            }

            var products = this.mongoDb.GetItemsFromCollection("Products");

            foreach (var product in products)
            {
                this.robotsFactoryData.Products.Add(new Product()
                {
                    ProductId = product["ProductId"].ToInt32(),
                    Name = product["ProductName"].ToString(),
                    Dimensions = product["Dimensions"].ToString(),
                    Weight = (decimal)product["Weight"].ToDouble(),
                    ProductTypeId = product["ProductTypeId"].ToInt32(),
                    Description = product["ShortDescription"].ToString(),
                    ManufacturerId = product["ManufacturerId"].ToInt32(),
                    ReleaseYear = product["ReleaseYear"].ToInt32(),
                    Price = (decimal)product["Price"].ToDouble()
                });
            }
        }

        public override void SaveChanges()
        {
            this.robotsFactoryData.SaveChanges();
        }
    }
}