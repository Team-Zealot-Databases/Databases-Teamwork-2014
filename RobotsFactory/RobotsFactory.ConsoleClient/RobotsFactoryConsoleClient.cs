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
            Console.Write("Loading...");

            try
            {
                using (var robotsFactoryContext = new RobotsFactoryContext())
                {
                    robotsFactoryContext.Database.Initialize(true);
                    robotsFactoryContext.Database.CommandTimeout = 5;

                    robotsFactoryContext.Countries.Add(new Country() { Name = "TestCountry" });
                    robotsFactoryContext.SaveChanges();

                    Console.Write("\r");

                    foreach (var country in robotsFactoryContext.Countries.ToList())
                    {
                        Console.WriteLine(country.Name);   
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}