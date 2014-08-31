namespace RobotsFactory.Data.Contracts
{
    using System;
    using System.Linq;
    using RobotsFactory.Models;

    public interface IRobotsFactoryData : IDisposable
    {
        IGenericRepository<Product> Products { get; }

        IGenericRepository<ProductType> ProductTypes { get; }

        IGenericRepository<Manufacturer> Manufacturers { get; }

        IGenericRepository<Address> Addresses { get; }

        IGenericRepository<City> Cities { get; }

        IGenericRepository<Country> Countries { get; }

        IGenericRepository<SalesReport> SalesReports { get; }

        IGenericRepository<SalesReportEntry> SalesReportEntries { get; }

        IGenericRepository<Store> Stores { get; }

        IGenericRepository<ManufacturerExpense> Expenses { get; }

        int SaveChanges();
    }
}