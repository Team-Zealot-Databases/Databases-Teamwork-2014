namespace RobotsFactory.SQLite
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class Item
    {
        [Key]
        public int ItemId { get; set; }

        public string Name { get; set; }

        public decimal Expense { get; set; }
    }
}