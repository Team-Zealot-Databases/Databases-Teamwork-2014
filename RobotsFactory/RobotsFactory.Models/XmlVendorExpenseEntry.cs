namespace RobotsFactory.Models
{
    using System;

    public class XmlVendorExpenseEntry
    {
        public XmlVendorExpenseEntry(string shopName, DateTime saleDate, decimal expense)
        {
            this.ShopName = shopName;
            this.SaleDate = saleDate;
            this.Expense = expense;
        }

        public string ShopName { get; set; }

        public DateTime SaleDate { get; set; }

        public decimal Expense { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\n   {1}\n   {2}", this.ShopName, this.SaleDate, this.Expense);
        }
    }
}