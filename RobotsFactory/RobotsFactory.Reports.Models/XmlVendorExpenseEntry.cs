namespace RobotsFactory.Reports.Models
{
    using System;

    public class XmlVendorExpenseEntry
    {
        public XmlVendorExpenseEntry(string manufacturerName, DateTime saleDate, decimal expense)
        {
            this.ManufacturerName = manufacturerName;
            this.SaleDate = saleDate;
            this.Expense = expense;
        }

        public string ManufacturerName { get; set; }

        public DateTime SaleDate { get; set; }

        public decimal Expense { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\n   {1}\n   {2}", this.ManufacturerName, this.SaleDate, this.Expense);
        }
    }
}