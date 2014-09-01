namespace RobotsFactory.Data.XmlProcessor
{
    using System.Linq;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.Models;
    using RobotsFactory.Reports.Models;

    public class ExpensesReportFactoryFromXmlData
    {
        private readonly IGenericRepository<Manufacturer> manufacturerRepository;

        public ExpensesReportFactoryFromXmlData(IGenericRepository<Manufacturer> manufacturerRepository)
        {
            this.manufacturerRepository = manufacturerRepository;
        }

        public ManufacturerExpense CreateStoreExpensesReport(XmlVendorExpenseEntry xmlVendorExpenseEntry)
        {
            var manufacturer = this.manufacturerRepository
                                   .All()
                                   .FirstOrDefault(s => s.Name == xmlVendorExpenseEntry.ManufacturerName);

            var expenseReport = new ManufacturerExpense()
            {
                ReportDate = xmlVendorExpenseEntry.SaleDate,
                Expense = xmlVendorExpenseEntry.Expense,
                ManufacturerId = manufacturer.ManufacturerId
            };

            return expenseReport;
        }
    }
}