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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IRobotsFactoryData robotsFactoryData = new RobotsFactoryData();
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
                zipFileProcessor.Extract(Constants.SampleReportsZipFilePath, Constants.ExtractedExcelReportsPath);

                var matchedDirectories = Utility.GetDirectoriesByPattern(Constants.ExtractedExcelReportsPath);
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
                var salesReportToPdfFactory = new PdfExportFactoryFromMsSqlDatabase(this.robotsFactoryData);
                salesReportToPdfFactory.ExportSalesEntriesToPdf(Constants.AggregatedSaleReportPdfPath, startDate, endDate);

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
                var xmlGenerator = new XmlReportGenerator(this.robotsFactoryData);
                xmlGenerator.CreateXmlReport(Constants.ExtractedXmlReportsPath, Constants.XmlReportName, startDate, endDate);

                this.ShowMessage("Sales reports were successfully exported as XML file...");
            }
            catch (Exception)
            {
                this.ShowMessage("Error! Cannot export reports as XML file...");
            }
        }

        private void OnReadAdditionalInformationFromXmlButtonClick(object sender, RoutedEventArgs e)
        {
            this.InitializeDatabaseConnectionIfNecessary();

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

                this.ShowMessage("Additional info from Xml was successfully added to MSSQL Server and MongoDb Cloud Database...");
            }
            catch (Exception)
            {
                this.ShowMessage("Error! Cannot export additional information from XML file to MSSQL Server and MongoDb...");
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
    }
}