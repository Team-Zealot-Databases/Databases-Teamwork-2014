namespace RobotsFactory.Reports.Models
{
    using System;
    using System.Linq;

    public class XmlSaleReport
    {
        public string ManufacturerName { get; set; }

        public DateTime ReportDate { get; set; }

        public decimal Sum { get; set; }
    }
}