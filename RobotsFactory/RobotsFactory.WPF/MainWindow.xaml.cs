namespace RobotsFactory.WPF
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using Microsoft.Win32;
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

        public MainWindow()
        {
            this.InitializeComponent();
        }
 
        private void OnReadFromMongoDbButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var mongoDbSeeder = new MongoDbSeeder(this.robotsFactoryData);
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
            var selectedPath = this.OpenFileDialogBox("zip");
            if (string.IsNullOrEmpty(selectedPath))
            {
                return;
            }

            try
            {
                var zipFileProcessor = new ZipFileProcessor();
                zipFileProcessor.Extract(selectedPath, Constants.ExtractedExcelReportsPath);

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
            var selectedPathAndFileName = this.SaveFileDialogBox("pdf", "Robots-Factory-Aggrerated-Sales-Report.pdf");
            if (selectedPathAndFileName == null)
            {
                return;
            }

            var startDate = this.GetSelectedDateOrDefault(this.startDateTimePicker.Text, DateTime.MinValue);
            var endDate = this.GetSelectedDateOrDefault(this.endDateTimePicker.Text, DateTime.Now);

            try
            {
                var salesReportToPdfFactory = new PdfExportFactoryFromMsSqlDatabase(this.robotsFactoryData);
                salesReportToPdfFactory.ExportSalesEntriesToPdf(selectedPathAndFileName.Item1, string.Empty, startDate, endDate);
                this.ShowMessage("Sales Report was successfully exported to PDF...");
            }
            catch (Exception)
            {
                this.ShowMessage("Error! Cannot export sales reports to Pdf file...");
            }
        }

        private void OnGenerateXmlReportButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedPathAndFileName = this.SaveFileDialogBox("xml", Constants.XmlReportName);
            if (selectedPathAndFileName == null)
            {
                return;
            }

            var startDate = this.GetSelectedDateOrDefault(this.startDateTimePicker.Text, DateTime.MinValue);
            var endDate = this.GetSelectedDateOrDefault(this.endDateTimePicker.Text, DateTime.Now);

            try
            {
                var xmlGenerator = new XmlReportGenerator(this.robotsFactoryData);
                xmlGenerator.CreateXmlReport(selectedPathAndFileName.Item1, string.Empty, startDate, endDate);

                this.ShowMessage("Sales reports were successfully exported as XML file...");
            }
            catch (Exception)
            {
                this.ShowMessage("Error! Cannot export reports as XML file...");
            }
        }

        private void OnReadAdditionalInformationFromXmlButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedPath = this.OpenFileDialogBox("xml");
            if (string.IsNullOrEmpty(selectedPath))
            {
                return;
            }

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

        private void OnOpenReportsDirectoryButtonClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Constants.ReportsDirectoryPath);
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
 
        private string OpenFileDialogBox(string typeOfFileToRead)
        {
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = Constants.ReportsDirectoryPath;
            this.SetFilter(dialog, typeOfFileToRead);

            var result = dialog.ShowDialog();
            if (result == true)
            {
                return dialog.FileName;
            }

            return string.Empty;
        }

        private Tuple<string, string> SaveFileDialogBox(string typeOfFileToSave, string defaultFileName)
        {
            var dialog = new SaveFileDialog();
            dialog.InitialDirectory = Constants.ReportsDirectoryPath;
            this.SetFilter(dialog, typeOfFileToSave, defaultFileName);

            var result = dialog.ShowDialog();
            if (result == true)
            {
                return new Tuple<string, string>(dialog.FileName, dialog.SafeFileName);
            }

            return null;
        }

        private void SetFilter(FileDialog dialog, string type, string defaultFileName = null)
        {
            switch (type)
            {
                case "zip":
                    {
                        dialog.DefaultExt = ".zip";
                        dialog.Filter = "Zip File (.zip)|*.zip";
                        break;
                    }

                case "pdf":
                    {
                        dialog.DefaultExt = ".zip";
                        dialog.Filter = "Pdf File (.pdf)|*.pdf";
                        break;
                    }

                case "xml":
                    {
                        dialog.DefaultExt = ".zip";
                        dialog.Filter = "XML File (.xml)|*.xml";
                        break;
                    }

                default:
                    {
                        dialog.FileName = "All files (*.*)|*.*";
                        break;
                    }
            }

            if (!string.IsNullOrEmpty(defaultFileName))
            {
                dialog.FileName = defaultFileName;
            }
        }
    }
}