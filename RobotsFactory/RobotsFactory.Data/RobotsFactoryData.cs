namespace RobotsFactory.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.Data.Repositories;
    using RobotsFactory.Models;

    public class RobotsFactoryData : IRobotsFactoryData
    {
        private readonly DbContext context;
        private readonly IDictionary<Type, object> repositories = new Dictionary<Type, object>();

        public RobotsFactoryData()
            : this(new RobotsFactoryContext())
        {
        }

        public RobotsFactoryData(DbContext context)
        {
            this.context = context;
        }

        public IGenericRepository<Product> Products
        {
            get
            {
                return this.GetRepository<Product>();
            }
        }

        public IGenericRepository<ProductType> ProductTypes
        {
            get
            {
                return this.GetRepository<ProductType>();
            }
        }

        public IGenericRepository<Manufacturer> Manufacturers
        {
            get
            {
                return this.GetRepository<Manufacturer>();
            }
        }

        public IGenericRepository<Address> Addresses
        {
            get
            {
                return this.GetRepository<Address>();
            }
        }

        public IGenericRepository<City> Cities
        {
            get
            {
                return this.GetRepository<City>();
            }
        }

        public IGenericRepository<Country> Countries
        {
            get
            {
                return this.GetRepository<Country>();
            }
        }

        public IGenericRepository<SalesReport> SalesReports
        {
            get
            {
                return this.GetRepository<SalesReport>();
            }
        }

        public IGenericRepository<SalesReportEntry> SalesReportEntries
        {
            get
            {
                return this.GetRepository<SalesReportEntry>();
            }
        }

        public IGenericRepository<Store> Stores
        {
            get
            {
                return this.GetRepository<Store>();
            }
        }

        public IGenericRepository<StoreExpense> Expenses
        {
            get
            {
                return this.GetRepository<StoreExpense>();
            }
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.context != null)
                {
                    this.context.Dispose();
                }
            }
        }

        private IGenericRepository<T> GetRepository<T>() where T : class
        {
            var typeOfModel = typeof(T);

            if (!this.repositories.ContainsKey(typeOfModel))
            {
                var typeOfRepository = typeof(GenericRepository<T>);
                this.repositories.Add(typeOfModel, Activator.CreateInstance(typeOfRepository, this.context));
            }

            return (IGenericRepository<T>)this.repositories[typeOfModel];
        }
    }
}