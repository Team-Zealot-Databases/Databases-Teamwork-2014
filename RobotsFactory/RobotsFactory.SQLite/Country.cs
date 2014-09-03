namespace RobotsFactory.SQLite
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class Country
    {
        [Key]
        public long CountryId { get; set; }

        public string Name { get; set; }

        public decimal TaxRate { get; set; }
    }
}