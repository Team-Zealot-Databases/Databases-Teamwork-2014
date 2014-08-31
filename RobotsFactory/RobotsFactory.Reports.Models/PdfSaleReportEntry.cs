namespace RobotsFactory.Reports.Models
{
    using System;
    using System.Linq;

    public class PdfSaleReportEntry
    {
        public string Name { get; set; }

        public int Quantity { get; set; }

        public DateTime Date { get; set; }

        public string Location { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Sum { get; set; }
    }
}