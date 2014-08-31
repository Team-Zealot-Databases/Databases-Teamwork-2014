namespace RobotsFactory.Reports.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ExcelReport
    {
        public ExcelReport()
        {
            this.Entries = new List<ExcelReportEntry>();
        }

        public string StoreName { get; set; }

        public DateTime ReportDate { get; set; }

        public decimal TotalSum { get; set; }

        public ICollection<ExcelReportEntry> Entries { get; set; }
    }
}