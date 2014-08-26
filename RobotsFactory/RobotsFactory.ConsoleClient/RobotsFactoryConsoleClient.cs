namespace RobotsFactory.ConsoleClient
{
    using System;
    using System.Linq;
    using RobotsFactory.Data;
    using RobotsFactory.Models;

    public class RobotsFactoryConsoleClient
    {
        public static void Main()
        {
            using (var robotsFactoryContext = new RobotsFactoryContext())
            {
                robotsFactoryContext.Database.Initialize(true);

                robotsFactoryContext.Countries.Add(new Country() { Name = "TestCountry" });
                robotsFactoryContext.SaveChanges();

                foreach (var country in robotsFactoryContext.Countries.ToList())
                {
                    Console.WriteLine(country.Name);   
                }
            }
        }
    }
}