namespace RobotsFactory.Data
{
    using System;
    using System.Linq;
    using RobotsFactory.Models;

    public class MongoDbSeeder : CentralDatabaseSeeder
    {
        private readonly MongoDbCloudDatabase mongoDb;
        private readonly RobotsFactoryContext context;

        public MongoDbSeeder(RobotsFactoryContext context)
            : this(context, new MongoDbCloudDatabase())
        {
        }

        public MongoDbSeeder(RobotsFactoryContext context, MongoDbCloudDatabase mongoDb)
        {
            this.context = context;
            this.mongoDb = mongoDb;
        }

        public override void AddCountries()
        {
            if (this.context.Countries.Any())
            {
                return;
            }

            var countries = this.mongoDb.GetItemsFromCollection("Countries");

            foreach (var country in countries)
            {
                this.context.Countries.Add(new Country()
                {
                    CountryId = country["CountryId"].ToInt32(),
                    Name = country["Name"].ToString()
                });
            }
        }

        public override void AddCities()
        {
            if (this.context.Cities.Any())
            {
                return;
            }

            var cities = this.mongoDb.GetItemsFromCollection("Cities");

            foreach (var city in cities)
            {
                this.context.Cities.Add(new City()
                {
                    CityId = city["CityId"].ToInt32(),
                    CountryId = city["CountryId"].ToInt32(),
                    Name = city["Name"].ToString()
                });
            }
        }

        public override void AddAddresses()
        {
            if (this.context.Addresses.Any())
            {
                return;
            }

            var addresses = this.mongoDb.GetItemsFromCollection("Addresses");

            foreach (var address in addresses)
            {
                this.context.Addresses.Add(new Address()
                {
                    AddressId = address["AddressId"].ToInt32(),
                    AddressText = address["AddressText"].ToString(),
                    CityId = address["CityId"].ToInt32(),
                });
            }
        }

        public override void AddManufacturers()
        {
            if (this.context.Manufacturers.Any())
            {
                return;
            }

            var manufacturers = this.mongoDb.GetItemsFromCollection("Manufacturers");

            foreach (var manufacturer in manufacturers)
            {
                this.context.Manufacturers.Add(new Manufacturer()
                {
                    ManufacturerId = manufacturer["ManufacturerId"].ToInt32(),
                    Name = manufacturer["Name"].ToString(),
                    AddressId = manufacturer["AddressId"].ToInt32(),
                });
            }
        }

        public override void AddProductTypes()
        {
            if (this.context.ProductTypes.Any())
            {
                return;
            }

            var productTypes = this.mongoDb.GetItemsFromCollection("ProductTypes");

            foreach (var productType in productTypes)
            {
                this.context.ProductTypes.Add(new ProductType()
                {
                    ProductTypeId = productType["ProductTypeId"].ToInt32(),
                    Name = productType["Name"].ToString()
                });
            }
        }

        public override void AddProducts()
        {
            if (this.context.Products.Any())
            {
                return;
            }

            var products = this.mongoDb.GetItemsFromCollection("Products");

            foreach (var product in products)
            {
                this.context.Products.Add(new Product()
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
            this.context.SaveChanges();
        }
    }
}