namespace RobotsFactory.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using RobotsFactory.Data.Migrations;
    using RobotsFactory.Models;

    public class RobotsFactoryContext : DbContext
    {
        const string RobotsFactoryDatabaseName = "RobotsFactory";

        public RobotsFactoryContext()
            : base(RobotsFactoryDatabaseName)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<RobotsFactoryContext, Configuration>());
        }

        public RobotsFactoryContext(string connectionString)
            : this()
        {
            this.Database.Connection.ConnectionString = connectionString;
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductType> ProductTypes { get; set; }

        public DbSet<Manufacturer> Manufacturers { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Country> Countries { get; set; }
    }
}