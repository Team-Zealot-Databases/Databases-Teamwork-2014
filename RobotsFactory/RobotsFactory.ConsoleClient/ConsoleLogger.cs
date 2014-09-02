namespace RobotsFactory.ConsoleClient
{
    using System;
    using System.Linq;
    using RobotsFactory.Data.Contracts;

    public class ConsoleLogger : ILogger
    {
        public void ShowMessage(string message)
        {
            Console.WriteLine("+ {0}{1}", message, Environment.NewLine);
        }
    }
}