namespace RobotsFactory.MongoDb.Mapping
{
    using System;
    using System.Linq;

    public class ManufacturerExpenseMap
    {
        public int ExpenseId { get; set; }

        public DateTime ReportDate { get; set; }

        public decimal Expense { get; set; }

        public int ManufacturerId { get; set; }
    }
}