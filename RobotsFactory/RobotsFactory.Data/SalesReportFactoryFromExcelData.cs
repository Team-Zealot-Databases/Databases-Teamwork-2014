namespace RobotsFactory.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RobotsFactory.Models;

    public class SalesReportFactoryFromExcelData
    {
        private readonly RobotsFactoryContext robotsFactoryContext;

        public SalesReportFactoryFromExcelData(RobotsFactoryContext robotsFactoryContext)
        {
            this.robotsFactoryContext = robotsFactoryContext;
        }

        public void CreateSalesReport(IList<string> excelData, string reportDateTime)
        {
            var storeName = excelData.First();
            var salesReportTotalSum = decimal.Parse(excelData.Last());

            var salesReportEntries = this.GetSalesReportEntries(excelData);

            var store = this.robotsFactoryContext.Stores.FirstOrDefault(s => s.Name == storeName);
            if (store == null)
            {
                store = new Store()
                {
                    Name = storeName
                };
            }

            var salesReport = new SalesReport()
            {
                ReportDate = DateTime.Parse(reportDateTime),
                SalesReportEntries = salesReportEntries,
                Store = store,
                TotalSum = salesReportTotalSum
            };

            this.robotsFactoryContext.SalesReports.Add(salesReport);
            this.robotsFactoryContext.SaveChanges();
        }

        public IList<SalesReportEntry> GetSalesReportEntries(IList<string> excelData)
        {
            var salesReportEntries = new List<SalesReportEntry>();

            for (int i = 1; i < excelData.Count - 2; i += 4)
            {
                salesReportEntries.Add(new SalesReportEntry()
                {
                    ProductId = int.Parse(excelData[i]),
                    Quantity = int.Parse(excelData[i + 1]),
                    UnitPrice = decimal.Parse(excelData[i + 2]),
                    Sum = decimal.Parse(excelData[i + 3])
                });
            }

            return salesReportEntries;
        }
    }
}