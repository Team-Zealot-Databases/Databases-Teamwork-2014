namespace RobotsFactory.Data
{
    using System;
    using System.Linq;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.Reports.Models;

    public class ReportQueries
    {
        private readonly IRobotsFactoryData robotsFactoryData;
     
        public ReportQueries(IRobotsFactoryData robotsFactoryData)
        {
            this.robotsFactoryData = robotsFactoryData;
        }

        /// <summary>
        // Query for getting all sold items information (product name, quantity, unit price, sum, date)
        /// </summary>
        public IQueryable<PdfSaleReportEntry> GetPdfSaleReportsFromDatabase(DateTime startDate, DateTime endDate)
        {
            var salesReportEntries = from sre in this.robotsFactoryData.SalesReportEntries.All()
                                     join pro in this.robotsFactoryData.Products.All() on sre.ProductId equals pro.ProductId
                                     join sl in this.robotsFactoryData.SalesReports.All() on sre.SalesReportId equals sl.SalesReportId
                                     where sl.ReportDate >= startDate && sl.ReportDate <= endDate
                                     orderby sl.ReportDate
                                     select new PdfSaleReportEntry
                                     {
                                         Name = pro.Name,
                                         Quantity = sre.Quantity,
                                         Date = sl.ReportDate,
                                         UnitPrice = sre.UnitPrice,
                                         Location = sl.Store.Name,
                                         Sum = sre.Sum
                                     };

            return salesReportEntries;
        }

        public IQueryable<XmlSaleReport> GetXmlSaleReportsFromDatabase(DateTime startDate, DateTime endDate)
        {
            var reportData = (from m in this.robotsFactoryData.Manufacturers.All()
                              join p in this.robotsFactoryData.Products.All() on m.ManufacturerId equals p.ManufacturerId
                              join s in this.robotsFactoryData.SalesReportEntries.All() on p.ProductId equals s.ProductId
                              join l in this.robotsFactoryData.SalesReports.All() on s.SalesReportId equals l.SalesReportId
                              where l.ReportDate >= startDate && l.ReportDate <= endDate
                              select new XmlSaleReport()
                              {
                                  ManufacturerName = m.Name,
                                  ReportDate = l.ReportDate,
                                  Sum = s.Sum
                              });

            return reportData;
        }

        public IQueryable<JsonProductsReportEntry> GetJsonProductsReportsFromDatabase()
        {
            var productsReportsEntries = from pro in this.robotsFactoryData.Products.All()
                                         join man in this.robotsFactoryData.Manufacturers.All() on pro.ManufacturerId equals man.ManufacturerId
                                         join sre in this.robotsFactoryData.SalesReportEntries.All() on pro.ProductId equals sre.ProductId
                                         group sre by new { ProductId = sre.ProductId, ProductName = pro.Name, ManufacturerName = man.Name }
                                         into grp select new JsonProductsReportEntry()
                                         {
                                             ProductId = grp.Key.ProductId,
                                             ProductName = grp.Key.ProductName,
                                             ManufacturerName = grp.Key.ManufacturerName,
                                             TotalQuantitySold = grp.Sum(x => x.Quantity),
                                             TotalIncome = grp.Sum(x => x.Sum)
                                         };

            return productsReportsEntries;
        }
    }
}