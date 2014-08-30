namespace RobotsFactory.ConsoleClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using RobotsFactory.Common;
    using RobotsFactory.Data;
    using RobotsFactory.Data.ExcelProcessor;
    using RobotsFactory.Data.MongoDb;
    using RobotsFactory.Data.PdfProcessor;

    public class RobotsFactoryConsoleClient
    {
        private const string SampleReportsZipFilePath = "../../../../Reports/Sales-Reports.zip";
        private const string AggregatedSaleReportPdfPath = "../../../../Reports/Robots-Factory-Aggrerated-Sales-Report.pdf";
        private const string ExtractedExcelReportsPath = @"../../../../Reports/Extracted_Reports";
        private const string ExtractedXmlReportsPath = @"../../../../Reports";
        private const string XmlReportName = @"xml-report.xml";

        public static void Main()
        {
            ConnectAndLoadDataFromMsSql();
        }

        private static void ConnectAndLoadDataFromMsSql()
        {
            using (var robotsFactoryContext = new RobotsFactoryContext())
            {
                robotsFactoryContext.Database.Initialize(true);
                //var xmlGen = new XmlReportGenerator(robotsFactoryContext);
                //xmlGen.GenerateXml(ExtractedXmlReportsPath, XmlReportName, "01.01.2012", "01.01.2015");
                //SeedDataFromMongoDB(robotsFactoryContext);
                //ExtractZipAndReadSalesReportExcelFiles(robotsFactoryContext);
                //ExportAggregatedSalesReportToPdf(robotsFactoryContext);
            }
            try
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
 
        private static void SeedDataFromMongoDB(RobotsFactoryContext robotsFactoryContext)
        {
            Console.WriteLine("1) Loading data from MongoDB Cloud Database and seed it in SQL Server...\n");

            var mongoDbSeeder = new MongoDbSeeder(robotsFactoryContext);
            mongoDbSeeder.Seed();
        }

        private static void ExtractZipAndReadSalesReportExcelFiles(RobotsFactoryContext robotsFactoryContext)
        {
            Console.WriteLine("2) Extracting files from .zip and reading Excel data...\n");

            var zipFileProcessor = new ZipFileProcessor();
            zipFileProcessor.Extract(SampleReportsZipFilePath, ExtractedExcelReportsPath);

            var matchedDirectories = Utility.GetDirectoriesByPattern(ExtractedExcelReportsPath);
            ReadExcelFilesAndCreateSalesReports(robotsFactoryContext, matchedDirectories);
        }
 
        private static void ExportAggregatedSalesReportToPdf(RobotsFactoryContext robotsFactoryContext)
        {
            Console.WriteLine("3) Exporting Sales Report to PDF...\n");

            var salesReportToPdfFactory = new PdfExportFactoryFromMsSqlDatabase(robotsFactoryContext);
            salesReportToPdfFactory.ExportSalesEntriesToPdf(AggregatedSaleReportPdfPath, new DateTime(2012, 1, 1), new DateTime(2014, 1, 1));
        }

        private static void ReadExcelFilesAndCreateSalesReports(RobotsFactoryContext robotsFactoryContext, IEnumerable<DirectoryInfo> matchedDirectories)
        {
            var salesReportFactory = new SalesReportFactoryFromExcelData(robotsFactoryContext);
            var excelDataReader = new ExcelDataReader();

            foreach (var dir in matchedDirectories)
            {
                foreach (var excelFile in dir.GetFiles("*.xls"))
                {
                    var excelData = excelDataReader.ReadSaleReportsData(excelFile.FullName);
                    salesReportFactory.CreateSalesReport(excelData, dir.Name);
                }
            }
        }

        private static string GetDateTimeAsString(string text, DateTime defaultValue)
        {
            if (string.IsNullOrEmpty(text))
            {
                return defaultValue.ToString("dd.MM.yyyy");
            }

            return DateTime.Parse(text).ToString("dd.MM.yyyy");
        }
    }
}