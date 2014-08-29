namespace RobotsFactory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using RobotsFactory.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<RobotsFactoryContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
            this.ContextKey = "RobotsFactory.Data.RobotsFactoryContext";
        }

        protected override void Seed(RobotsFactoryContext context)
        {
            //var mongoDb = new MongoDbCloudDatabase();
            //this.AddCountries(context, mongoDb);
            //this.AddCities(context, mongoDb);
        }
 
        private void AddCountries(RobotsFactoryContext context, MongoDbCloudDatabase mongoDb)
        {
            if (context.Countries.Any())
            {
                return;
            }

            var countries = mongoDb.GetItemsFromCollection("Countries");

            foreach (var country in countries)
            {
                context.Countries.Add(new Country()
                {
                    CountryId = (int)country["CountryId"],
                    Name = country["Name"].ToString()
                });
            }
        }

        private void AddCities(RobotsFactoryContext context, MongoDbCloudDatabase mongoDb)
        {
            if (context.Cities.Any())
            {
                return;
            }

            var cities = mongoDb.GetItemsFromCollection("Cities");

            foreach (var city in cities)
            {
                context.Cities.Add(new City()
                {
                    CityId = (int)city["CityId"],
                    CountryId = (int)city["CountryId"],
                    Name = city["Name"].ToString()
                });
            }
        }
    }
}