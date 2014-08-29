namespace RobotsFactory.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class SalesReport
    {
        private ICollection<SalesReportEntry> salesReportEntries;

        public SalesReport()
        {
            this.salesReportEntries = new HashSet<SalesReportEntry>();
        }

        [Key]
        public int SalesReportId { get; set; }

        public DateTime ReportDate { get; set; }

        public decimal TotalSum { get; set; }

        [ForeignKey("Manufacturer")]
        public int ManufacturerId { get; set; }

        public virtual Manufacturer Manufacturer { get; set; }

        public virtual ICollection<SalesReportEntry> SalesReportEntries
        {
            get
            {
                return this.salesReportEntries;
            }
            set
            {
                this.salesReportEntries = value;
            }
        }
    }
}