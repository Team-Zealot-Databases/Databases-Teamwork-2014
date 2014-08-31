namespace RobotsFactory.Data.Contracts
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using RobotsFactory.Models;

    public interface IRobotsFactoryDbContext : IDbContext
    {
        IDbSet<Product> Products { get; set; }

        IDbSet<ProductType> ProductTypes { get; set; }

        IDbSet<Manufacturer> Manufacturers { get; set; }

        IDbSet<Address> Addresses { get; set; }

        IDbSet<City> Cities { get; set; }

        IDbSet<Country> Countries { get; set; }

        IDbSet<SalesReport> SalesReports { get; set; }

        IDbSet<SalesReportEntry> SalesReportEntries { get; set; }

        IDbSet<Store> Stores { get; set; }

        IDbSet<StoreExpense> Expenses { get; set; }
    }
}