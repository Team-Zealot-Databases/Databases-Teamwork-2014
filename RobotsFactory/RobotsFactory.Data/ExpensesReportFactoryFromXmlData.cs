namespace RobotsFactory.Data
{
    using System.Linq;
    using RobotsFactory.Models;
    using RobotsFactory.Data.XMLProcessor;

    public class ExpensesReportFactoryFromXmlData
    {
        private readonly RobotsFactoryContext robotsFactoryContext;

        public ExpensesReportFactoryFromXmlData(RobotsFactoryContext robotsFactoryContext)
        {
            this.robotsFactoryContext = robotsFactoryContext;
        }

        public void CreateExpensesReport(XmlData xmlData)
        {
            var store = this.robotsFactoryContext.Stores.FirstOrDefault(s => s.Name == xmlData.ShopName);

            if (store == null)
            {
                store = new Store()
                {
                    Name = xmlData.ShopName
                };
            }

            var expenseReport = new StoreExpense()
            {
                ReportDate = xmlData.SaleDate,
                Expense = xmlData.Expense,
                Store = store,
            };

            this.robotsFactoryContext.Expenses.Add(expenseReport);
            this.robotsFactoryContext.SaveChanges();
        }
    }
}