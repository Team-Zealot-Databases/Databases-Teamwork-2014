namespace RobotsFactory.WPF
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using RobotsFactory.Common;
    using RobotsFactory.Data;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.Data.ExcelReader;
    using RobotsFactory.Data.MongoDb;
    using RobotsFactory.Data.PdfProcessor;
    using RobotsFactory.Data.XmlProcessor;
    using RobotsFactory.WPF.Contracts;

    public class RobotsFactoryModule
    {
        private readonly IRobotsFactoryData robotsFactoryData = new RobotsFactoryData();
        private readonly ILogger logger;

        public RobotsFactoryModule(ILogger logger)
        {
            this.logger = logger;
        }

        public void ReadFromMongoDb()
        {
            try
            {
                var mongoDbSeeder = new MongoDbSeeder(this.robotsFactoryData);
                mongoDbSeeder.Seed();

                this.logger.ShowMessage("Data from MongoDB Cloud Database was successfully seeded in SQL Server...");
            }
            catch (Exception ex)
            {
                this.logger.ShowMessage("Error! Cannot read data from MongoDb Cloud Database...");
                MessageBox.Show("A handled exception just occurred: " + ex.Message, "Something bad happened!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void ReadSaleReportsFromExcel(string selectedPath)
        {
            try
            {
                var zipFileProcessor = new ZipFileProcessor();
                zipFileProcessor.Extract(selectedPath, Constants.ExtractedExcelReportsPath);

                var matchedDirectories = Utility.GetDirectoriesByPattern(Constants.ExtractedExcelReportsPath);
                this.ParseExcelDataAndExportItInSqlServer(matchedDirectories);

                this.logger.ShowMessage("Sales reports from Excel files were successfully imported to MSSQL...");
            }
            catch (Exception ex)
            {
                this.logger.ShowMessage("Error! Cannot extract and import sales report data from Excel files...");
                MessageBox.Show("A handled exception just occurred: " + ex.Message, "Something bad happened!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void ReadAdditionalInformation(string selectedPath)
        {
            try
            {
                var expensesFactory = new ExpensesReportFactoryFromXmlData(this.robotsFactoryData.Manufacturers);
                var xmlReader = new XmlDataReader();
                var xmlData = xmlReader.ReadXmlReportsData(Constants.XmlFilePath);

                foreach (var expenseLog in xmlData)
                {
                    var storeExpenseReport = expensesFactory.CreateStoreExpensesReport(expenseLog);
                    this.robotsFactoryData.Expenses.Add(storeExpenseReport);
                }

                this.robotsFactoryData.SaveChanges();

                this.logger.ShowMessage("Additional info from Xml was successfully added to MSSQL Server and MongoDb Cloud Database...");
            }
            catch (Exception ex)
            {
                this.logger.ShowMessage("Error! Cannot export additional information from XML file to MSSQL Server and MongoDb...");
                MessageBox.Show("A handled exception just occurred: " + ex.Message, "Something bad happened!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void ExportAggregatedSalesReportToPdf(Tuple<string, string> selectedPathAndFileName, DateTime startDate, DateTime endDate)
        {
            try
            {
                var salesReportToPdfFactory = new PdfExportFactoryFromMsSqlDatabase(this.robotsFactoryData);
                salesReportToPdfFactory.ExportSalesEntriesToPdf(selectedPathAndFileName.Item1, string.Empty, startDate, endDate);
                this.logger.ShowMessage("Sales Report was successfully exported to PDF...");
            }
            catch (Exception ex)
            {
                this.logger.ShowMessage("Error! Cannot export sales reports to Pdf file...");
                MessageBox.Show("A handled exception just occurred: " + ex.Message, "Something bad happened!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void GenerateXmlReport(Tuple<string, string> selectedPathAndFileName, DateTime startDate, DateTime endDate)
        {
            try
            {
                var xmlGenerator = new XmlReportGenerator(this.robotsFactoryData);
                xmlGenerator.CreateXmlReport(selectedPathAndFileName.Item1, string.Empty, startDate, endDate);

                this.logger.ShowMessage("Sales reports were successfully exported as XML file...");
            }
            catch (Exception ex)
            {
                this.logger.ShowMessage("Error! Cannot export reports as XML file...");
                MessageBox.Show("A handled exception just occurred: " + ex.Message, "Something bad happened!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ParseExcelDataAndExportItInSqlServer(IEnumerable<DirectoryInfo> matchedDirectories)
        {
            var salesReportFactory = new SalesReportGeneratorFromExcel(this.robotsFactoryData.Stores);
            var excelSaleReportReader = new ExcelSaleReportReader();

            foreach (var dir in matchedDirectories)
            {
                foreach (var excelFile in dir.GetFiles(Constants.ExcelFileExtensionPattern))
                {
                    var excelData = excelSaleReportReader.CreateSaleReport(excelFile.FullName, dir.Name);
                    var salesReport = salesReportFactory.CreateSalesReport(excelData, dir.Name);
                    this.robotsFactoryData.SalesReports.Add(salesReport);
                }
            }

            this.robotsFactoryData.SaveChanges();
        }
    }
}