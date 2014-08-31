namespace RobotsFactory.Reports.Models
{
    using System;
    using System.Linq;

    public class ExcelReportEntry
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Sum { get; set; }
    }
}