namespace RobotsFactory.Data.XmlProcessor
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using RobotsFactory.Common;
    using RobotsFactory.Data.Contracts;

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
    
                var manufacturerData = (from m in this.robotsFactoryData.Manufacturers.All()
                                        join p in this.robotsFactoryData.Products.All() on m.ManufacturerId equals p.ManufacturerId
                                        join s in this.robotsFactoryData.SalesReportEntries.All() on p.ProductId equals s.ProductId
                                        join l in this.robotsFactoryData.SalesReports.All() on s.SalesReportId equals l.SalesReportId
                                        where l.ReportDate >= startDate && l.ReportDate <= endDate
                                        select new
                                        {
                                            Date = l.ReportDate,
                                            Name = m.Name,
                                            Sum = s.Sum
                                        })
                                          .GroupBy(a => new { a.Name, a.Date })
                                          .GroupBy(a => new { Name = a.Key.Name })
                                          .Select(a => new
                                          {
                                              Name = a.Key.Name,
                                              Reports = a
                                          });

                foreach (var manufacturer in manufacturerData)
                {
                    this.SetTitle(writer, manufacturer.Name);

                    foreach (var report in manufacturer.Reports)
                    {
                        this.WriteSummaryToSale(writer, report.Key.Date, report.Sum(s => s.Sum));
                    }

                    writer.WriteEndElement();
                }
            }
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