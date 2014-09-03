namespace RobotsFactory.SQLite
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class SQLiteDbContext : DbContext
    {
        public SQLiteDbContext()
            : base("RobotsFactorySqlite")
        {
            //this.Database.Connection.ConnectionString = @"Data Source=..\..\CountryTaxesDb.sqlite";
        }

        public DbSet<Country> Countries { get; set; }
    }
}