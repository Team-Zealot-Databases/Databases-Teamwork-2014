namespace RobotsFactory.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.Models;
    using RobotsFactory.Reports.Models;

    public class SalesReportGeneratorFromExcel
    {
        private readonly IGenericRepository<Store> storeRepository;

        public SalesReportGeneratorFromExcel(IGenericRepository<Store> storeRepository)
        {
            this.storeRepository = storeRepository;
        }

        public SalesReport CreateSalesReport(ExcelReport excelReport, string reportDateTime)
        {
            var salesReportEntries = this.GetSalesReportEntries(excelReport.Entries);
            var store = this.GetOrCreateStore(excelReport.StoreName);
            var salesReport = new SalesReport()
            {
                ReportDate = excelReport.ReportDate,
                SalesReportEntries = salesReportEntries,
                Store = store,
                TotalSum = excelReport.TotalSum
            };

            return salesReport;
        }

        private ICollection<SalesReportEntry> GetSalesReportEntries(ICollection<ExcelReportEntry> excelReportEntries)
        {
            var salesReportEntries = excelReportEntries.Select(e => new SalesReportEntry()
            {
                ProductId = e.ProductId,
                Quantity = e.Quantity,
                UnitPrice = e.UnitPrice,
                Sum = e.Sum
            });
 
            return salesReportEntries.ToList();
        }

        private Store GetOrCreateStore(string storeName)
        {
            var store = this.storeRepository.All().FirstOrDefault(s => s.Name == storeName);

            if (store == null)
            {
                store = new Store()
                {
                    Name = storeName
                };
            }

            return store;
        }
    }
}