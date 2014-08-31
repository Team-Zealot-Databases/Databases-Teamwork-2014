namespace RobotsFactory.ConsoleClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using RobotsFactory.Common;
    using RobotsFactory.Data;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.Data.ExcelReader;
    using RobotsFactory.Data.MongoDb;
    using RobotsFactory.Data.PdfProcessor;
    using RobotsFactory.Data.XmlProcessor;

    public class RobotsFactoryConsoleClient
    {
        private static readonly IRobotsFactoryData robotsFactoryData = new RobotsFactoryData();

        public static void Main()
        {
            ConnectAndLoadDataFromMsSql();
        }

        private static void ConnectAndLoadDataFromMsSql()
        {
            using (var robotsFactoryContext = new RobotsFactoryContext())
            {
                robotsFactoryContext.Database.Initialize(true);
                SeedDataFromMongoDB(robotsFactoryContext);
                ExtractZipAndReadSalesReportExcelFiles();
                ExportAggregatedSalesReportToPdf();
                ReadXmlFileAndAddReport();
                ExportXmlReportForManufacturersSales();

                Console.WriteLine("-> Program finish sucessfully...\n");
            }
        }
     
        private static void SeedDataFromMongoDB(RobotsFactoryContext robotsFactoryContext)
        {
            Console.WriteLine("1) Loading data from MongoDB Cloud Database and seed it in SQL Server...\n");

            var mongoDbSeeder = new MongoDbSeeder(robotsFactoryContext);
            mongoDbSeeder.Seed();
        }

        private static void ExtractZipAndReadSalesReportExcelFiles()
        {
            Console.WriteLine("2) Extracting files from .zip and reading Excel data...\n");

            var zipFileProcessor = new ZipFileProcessor();
            zipFileProcessor.Extract(Constants.SampleReportsZipFilePath, Constants.ExtractedExcelReportsPath);

            var matchedDirectories = Utility.GetDirectoriesByPattern(Constants.ExtractedExcelReportsPath);
            ReadExcelFilesAndCreateSalesReports(matchedDirectories);
        }
 
        private static void ExportAggregatedSalesReportToPdf()
        {
            Console.WriteLine("3) Exporting Sales Report to PDF file...\n");

            var salesReportToPdfFactory = new PdfExportFactoryFromMsSqlDatabase(robotsFactoryData);
            salesReportToPdfFactory.ExportSalesEntriesToPdf(Constants.AggregatedSaleReportPdfPath, new DateTime(2012, 1, 1), new DateTime(2014, 1, 1));
        }

        private static void ReadXmlFileAndAddReport()
        {
            Console.WriteLine("4) Reading additional information from XML file and import data to MSSQL Database...\n");

            var expensesFactory = new ExpensesReportFactoryFromXmlData(robotsFactoryData.Manufacturers);
            var xmlReader = new XmlDataReader();
            var xmlData = xmlReader.ReadXmlReportsData(Constants.XmlFilePath);

            foreach (var expenseLog in xmlData)
            {
                var storeExpenseReport = expensesFactory.CreateStoreExpensesReport(expenseLog);
                robotsFactoryData.Expenses.Add(storeExpenseReport);
            }

            robotsFactoryData.SaveChanges();
        }

        private static void ExportXmlReportForManufacturersSales()
        {
            Console.WriteLine("5) Exporting XML Report...\n");
  
            var xmlGenerator = new XmlReportGenerator(robotsFactoryData);
            xmlGenerator.CreateXmlReport(Constants.ExtractedXmlReportsPath, Constants.XmlReportName, new DateTime(2012, 1, 1), new DateTime(2014, 1, 1));
        }

        private static void ReadExcelFilesAndCreateSalesReports(IEnumerable<DirectoryInfo> matchedDirectories)
        {
            var salesReportFactory = new SalesReportGeneratorFromExcel(robotsFactoryData.Stores);
            var excelSaleReportReader = new ExcelSaleReportReader();

            foreach (var dir in matchedDirectories)
            {
                foreach (var excelFile in dir.GetFiles(Constants.ExcelFileExtensionPattern))
                {
                    var excelData = excelSaleReportReader.CreateSaleReport(excelFile.FullName, dir.Name);
                    var salesReport = salesReportFactory.CreateSalesReport(excelData, dir.Name);
                    robotsFactoryData.SalesReports.Add(salesReport);
                }
            }

            robotsFactoryData.SaveChanges();
        }
    }
}