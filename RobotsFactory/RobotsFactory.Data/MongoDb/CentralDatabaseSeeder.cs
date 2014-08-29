namespace RobotsFactory.Data.MongoDb
{
    using System;
    using System.Linq;

    public abstract class CentralDatabaseSeeder
    {
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

        public abstract void AddCountries();

        public abstract void AddCities();

        public abstract void AddAddresses();

        public abstract void AddManufacturers();

        public abstract void AddProductTypes();

        public abstract void AddProducts();

        public abstract void SaveChanges();
    }
}