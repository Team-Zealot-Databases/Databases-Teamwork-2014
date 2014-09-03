namespace RobotsFactory.ConsoleClient
{
    using System;
    using System.Linq;
    using RobotsFactory.Common;
    using RobotsFactory.Data;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.MySQL;

    public class RobotsFactoryConsoleClient
    {
        private static readonly DateTime reportStartDate = new DateTime(2012, 1, 1);
        private static readonly DateTime reportEndDate = new DateTime(2014, 1, 1);
        private static readonly Tuple<string, string> pdfFileInfo = Tuple.Create(Constants.AggregatedSaleReportPdfPath, Constants.PdfReportName);
        private static readonly Tuple<string, string> xmlReportFileInfo = Tuple.Create(Constants.ExtractedXmlReportsPath, Constants.XmlReportName);

        private static RobotsFactoryModule robotsFactoryModule;
        private static ILogger logger;

        public static void Main()
        {
            //InitializeComponent();

            //SeedDataFromMongoDB();
            //ExtractZipAndReadSalesReportExcelFiles();
            //ExportAggregatedSalesReportToPdf();
            //ReadXmlFileAndAddReport();
            //ExportXmlReportForManufacturersSales();

            //Console.WriteLine("-> Program finish sucessfully...\n");

            var robotsFactoryMySqlContext = new RobotsFactoryMySqlContext();

            robotsFactoryMySqlContext.Add(new JsonReport()
            {
                JsonContent = DateTime.Now.ToString(),
                JsonFileName = DateTime.Now.ToString()
            });

            robotsFactoryMySqlContext.SaveChanges();

            foreach (var jsonReport in robotsFactoryMySqlContext.JsonReports)
            {
                Console.WriteLine("{0} | {1} | {2}", jsonReport.ReportId, jsonReport.JsonContent, jsonReport.JsonFileName);
            }
        }
     
        private static void SeedDataFromMongoDB()
        {
            robotsFactoryModule.ReadFromMongoDb();
        }

        private static void ExtractZipAndReadSalesReportExcelFiles()
        {
            robotsFactoryModule.ReadSaleReportsFromExcel(Constants.SampleReportsZipFilePath);
        }
 
        private static void ExportAggregatedSalesReportToPdf()
        {
            robotsFactoryModule.ExportAggregatedSalesReportToPdf(pdfFileInfo, reportStartDate, reportEndDate);
        }

        private static void ReadXmlFileAndAddReport()
        {
            robotsFactoryModule.ReadAdditionalInformation(Constants.XmlFilePath);
        }

        private static void ExportXmlReportForManufacturersSales()
        {
            robotsFactoryModule.GenerateXmlReport(xmlReportFileInfo, reportStartDate, reportEndDate);
        }

        private static void InitializeComponent()
        {
            logger = new ConsoleLogger();
            robotsFactoryModule = new RobotsFactoryModule(logger);
        }
    }
}