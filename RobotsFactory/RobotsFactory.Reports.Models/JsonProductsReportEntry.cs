namespace RobotsFactory.Reports.Models
{
    using System;
    using System.Linq;

    public class JsonProductsReportEntry
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ManufacturerName { get; set; }

        public decimal TotalQuantitySold { get; set; }

        public decimal TotalIncome { get; set; }
    }
}
