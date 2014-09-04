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
        }

        public DbSet<Item> Items { get; set; }
    }
}