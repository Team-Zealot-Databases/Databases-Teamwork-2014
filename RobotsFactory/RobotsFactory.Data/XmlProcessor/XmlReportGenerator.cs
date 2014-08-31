namespace RobotsFactory.Data.XmlProcessor
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using RobotsFactory.Common;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.Reports.Models;

    public class XmlReportGenerator
    {
        private const string DateTimeFormatInXml = "dd-MMM-yyyy";
        private const string EncodingType = "utf-8";

        private readonly IRobotsFactoryData robotsFactoryData;

        public XmlReportGenerator(IRobotsFactoryData robotsFactoryData)
        {
            this.robotsFactoryData = robotsFactoryData;
        }

        public void CreateXmlReport(string pathToSave, string xmlReportName, DateTime startDate, DateTime endDate)
        {
            var encoding = Encoding.GetEncoding(EncodingType);
            Utility.CreateDirectoryIfNotExists(pathToSave);

            using (var writer = new XmlTextWriter(pathToSave + xmlReportName, encoding))
            {
                this.SetHeader(writer);
                var reportData = this.GetSaleReportsFromDatabase(startDate, endDate);

                foreach (var manufacturer in reportData.Select(a => new { Name = a.Key, Reports = a.GroupBy(b => b.ReportDate) }))
                {
                    this.SetTitle(writer, manufacturer.Name);

                    foreach (var report in manufacturer.Reports)
                    {
                        this.WriteSummaryToSale(writer, report.Key, report.Sum(s => s.Sum));
                    }

                    writer.WriteEndElement();
                }
            }
        }
 
        private IQueryable<IGrouping<string, XmlSaleReport>> GetSaleReportsFromDatabase(DateTime startDate, DateTime endDate)
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
                              })
                                .GroupBy(a => a.ManufacturerName);

            return reportData;
        }
 
        private void SetHeader(XmlTextWriter writer)
        {
            writer.Formatting = Formatting.Indented;
            writer.IndentChar = '\t';
            writer.Indentation = 1;

            writer.WriteStartDocument();
            writer.WriteStartElement("sales");
            writer.WriteAttributeString("name", "Sales Report");
        }
 
        private void SetTitle(XmlTextWriter writer, string vendorName)
        {
            writer.WriteStartElement("sale");
            writer.WriteAttributeString("vendor", vendorName);
        }
 
        private void WriteSummaryToSale(XmlTextWriter writer, DateTime date, decimal totalSum)
        {
            writer.WriteStartElement("summary");
            writer.WriteAttributeString("date", date.ToString(DateTimeFormatInXml));
            writer.WriteAttributeString("total-sum", totalSum.ToString());
            writer.WriteEndElement();
        }
    }
}