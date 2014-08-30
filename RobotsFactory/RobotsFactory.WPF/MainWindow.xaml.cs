namespace RobotsFactory.WPF
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using RobotsFactory.Common;
    using RobotsFactory.Data;
    using RobotsFactory.Data.ExcelProcessor;
    using RobotsFactory.Data.MongoDb;
    using RobotsFactory.Data.PdfProcessor;
    using RobotsFactory.Data.XmlProcessor;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string SampleReportsZipFilePath = "../../../../Reports/Sales-Reports.zip";
        private const string AggregatedSaleReportPdfPath = "../../../../Reports/Robots-Factory-Aggrerated-Sales-Report.pdf";
        private const string ExtractedExcelReportsPath = @"../../../../Reports/Excel_Reports/";
        private const string ExtractedXmlReportsPath = @"../../../../Reports/XML_Reports/";
        private const string XmlReportName = @"xml-report.xml";

        private RobotsFactoryContext robotsFactoryContext;

        public MainWindow()
        {
            this.InitializeComponent();
        }
 
        private void InitializeDatabaseConnectionIfNecessary()
        {
            this.robotsFactoryContext = new RobotsFactoryContext();
            this.robotsFactoryContext.Database.Initialize(true);
        }

        private void OnReadFromMongoDbButtonClick(object sender, RoutedEventArgs e)
        {
            this.InitializeDatabaseConnectionIfNecessary();

            try
            {
                var mongoDbSeeder = new MongoDbSeeder(this.robotsFactoryContext);
                mongoDbSeeder.Seed();

                this.ShowMessage("Data from MongoDB Cloud Database was successfully seeded in SQL Server...");
            }
            catch (Exception)
            {
                this.ShowMessage("Error! Cannot read data from MongoDb Cloud Database...");
            }
        }

        private void OnReadSaleReportsFromExcelButtonClick(object sender, RoutedEventArgs e)
        {
            this.InitializeDatabaseConnectionIfNecessary();

            try
            {
                var zipFileProcessor = new ZipFileProcessor();
                zipFileProcessor.Extract(SampleReportsZipFilePath, ExtractedExcelReportsPath);

                var matchedDirectories = Utility.GetDirectoriesByPattern(ExtractedExcelReportsPath);
                this.ParseExcelDataAndExportItInSqlServer(matchedDirectories);

                this.ShowMessage("Sales reports from Excel files were successfully imported to MSSQL...");
            }
            catch (Exception)
            {
                this.ShowMessage("Error! Cannot extract and import sales report data from Excel files...");
            }
        }

        private void OnExportAggregatedSalesReportToPdfButtonClick(object sender, RoutedEventArgs e)
        {
            this.InitializeDatabaseConnectionIfNecessary();
            var startDate = this.GetSelectedDateOrDefault(this.startDateTimePicker.Text, DateTime.MinValue);
            var endDate = this.GetSelectedDateOrDefault(this.endDateTimePicker.Text, DateTime.Now);

            try
            {
                var salesReportToPdfFactory = new PdfExportFactoryFromMsSqlDatabase(this.robotsFactoryContext);
                salesReportToPdfFactory.ExportSalesEntriesToPdf(AggregatedSaleReportPdfPath, startDate, endDate);

                this.ShowMessage("Sales Report was successfully exported to PDF...");
            }
            catch (Exception)
            {
                this.ShowMessage("Error! Cannot export sales reports to Pdf file...");
            }
        }

        private void OnGenerateXmlReportButtonClick(object sender, RoutedEventArgs e)
        {
            this.InitializeDatabaseConnectionIfNecessary();
            var startDate = this.GetSelectedDateOrDefault(this.startDateTimePicker.Text, DateTime.MinValue);
            var endDate = this.GetSelectedDateOrDefault(this.endDateTimePicker.Text, DateTime.Now);

            try
            {
                var xmlGenerator = new XmlReportGenerator(this.robotsFactoryContext);
                xmlGenerator.GenerateXml(ExtractedXmlReportsPath, XmlReportName, startDate, endDate);

                this.ShowMessage("Sales reports were successfully exported as XML file...");
            }
            catch (Exception)
            {
                this.ShowMessage("Error! Cannot export reports as XML file...");
            }
        }

        private DateTime GetSelectedDateOrDefault(string text, DateTime defaultValue)
        {
            if (string.IsNullOrEmpty(text))
            {
                return defaultValue;
            }

            return DateTime.Parse(text);
        }

        private void ParseExcelDataAndExportItInSqlServer(IEnumerable<DirectoryInfo> matchedDirectories)
        {
            var salesReportFactory = new SalesReportFactoryFromExcelData(this.robotsFactoryContext);
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

        private void ShowMessage(string text)
        {
            this.messageBox.Text = text;
        }

        private void ClearMessages()
        {
            this.messageBox.Text = string.Empty;            
        }

        private void ToggleAllButtons()
        {
            this.generatePdfReportsButton.IsEnabled = !this.generatePdfReportsButton.IsEnabled;
            this.mongoDbButton.IsEnabled = !this.mongoDbButton.IsEnabled;
            this.readExcelReportsButton.IsEnabled = !this.readExcelReportsButton.IsEnabled;
        }

        private void OnWindowFormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.CloseConnection();
        }

        private void OnWindowFormClosed(object sender, EventArgs e)
        {
            this.CloseConnection();
        }

        private void CloseConnection()
        {
            if (this.robotsFactoryContext != null)
            {
                this.robotsFactoryContext.Dispose();
            }
        }
    }
}