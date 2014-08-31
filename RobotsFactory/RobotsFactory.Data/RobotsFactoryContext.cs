namespace RobotsFactory.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.Data.Migrations;
    using RobotsFactory.Models;

    public class RobotsFactoryContext : DbContext, IRobotsFactoryDbContext
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

        public IDbSet<Product> Products { get; set; }

        public IDbSet<ProductType> ProductTypes { get; set; }

        public IDbSet<Manufacturer> Manufacturers { get; set; }

        public IDbSet<Address> Addresses { get; set; }

        public IDbSet<City> Cities { get; set; }

        public IDbSet<Country> Countries { get; set; }

        public IDbSet<SalesReport> SalesReports { get; set; }

        public IDbSet<SalesReportEntry> SalesReportEntries { get; set; }

        public IDbSet<Store> Stores { get; set; }

        public IDbSet<ManufacturerExpense> Expenses { get; set; }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        public new void SaveChanges()
        {
            base.SaveChanges();
        }
    }
}