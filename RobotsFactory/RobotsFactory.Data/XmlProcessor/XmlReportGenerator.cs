using System;
using System.Data;
using System.Data.SqlClient;
namespace RobotsFactory.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;

    public class XmlReportGenerator
    {
        public void GenerateXml(RobotsFactoryContext context)
        {

            var reportsData = context.SalesReports.Select(s =>
                new
                {
                    Vendor = s.Store.Name,
                    Date = s.ReportDate,
                    TotalSum = s.TotalSum
                })
                .OrderBy(d => d.Date)
                .GroupBy(v => v.Vendor);

            string fileName = "../../sales-report.xml";
            Encoding encoding = Encoding.GetEncoding("utf-8");
            using (XmlTextWriter writer = new XmlTextWriter(fileName, encoding))
            {
                writer.Formatting = Formatting.Indented;
                writer.IndentChar = '\t';
                writer.Indentation = 1;

                writer.WriteStartDocument();
                writer.WriteStartElement("sales");
                writer.WriteAttributeString("name", "Sales Report");

                var data = (from m in context.Manufacturers
                            join p in context.Products on m.ManufacturerId equals p.ManufacturerId
                            join s in context.SalesReportEntries on p.ProductId equals s.ProductId
                            join l in context.SalesReports on s.SalesReportId equals l.SalesReportId
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
                           })
                           .ToList();

                foreach (var manufacturer in data)
                {
                    Console.WriteLine(manufacturer.Name);
                    writer.WriteStartElement("sale");
                    writer.WriteAttributeString("vendor", manufacturer.Name);

                    foreach (var report in manufacturer.Reports)
                    {
                        Console.WriteLine(report.Key.Date + " " + report.Sum(a => a.Sum));
                        WriteSummaryToSale(writer, report.Key.Date.ToString("dd-MMM-yyyy"), report.Sum(s => s.Sum));
                    }

                    writer.WriteEndElement();
                    Console.WriteLine();
                }
            }
        }

        private static void WriteSummaryToSale(XmlTextWriter writer, string date, decimal totalSum)
        {
            writer.WriteStartElement("summary");
            writer.WriteAttributeString("date", date);
            writer.WriteAttributeString("total-sum", totalSum.ToString());
            writer.WriteEndElement();
        }
    }
}
