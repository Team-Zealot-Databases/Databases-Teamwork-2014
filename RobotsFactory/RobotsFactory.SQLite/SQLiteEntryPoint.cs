namespace RobotsFactory.SQLite
{
    using System;
    using System.Linq;

    public class SQLiteEntryPoint
    {
        public static void Main(string[] args)
        {
            var sqliteDbContext = new SQLiteDbContext();
            sqliteDbContext.Countries.Add(new Country()
            {
                Name = DateTime.Now.ToString(),
                TaxRate = 123.123m
            });

            sqliteDbContext.SaveChanges();

            foreach (var country in sqliteDbContext.Countries)
            {
                Console.WriteLine("{0} | {1} | {2}", country.CountryId, country.Name, country.TaxRate);
            }
        }
    }
}