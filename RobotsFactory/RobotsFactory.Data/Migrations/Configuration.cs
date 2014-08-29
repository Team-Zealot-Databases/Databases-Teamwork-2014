namespace RobotsFactory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using RobotsFactory.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<RobotsFactoryContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
            this.ContextKey = "RobotsFactory.Data.RobotsFactoryContext";
        }
    }
}