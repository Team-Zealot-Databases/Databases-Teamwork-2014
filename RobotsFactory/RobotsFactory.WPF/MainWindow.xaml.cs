﻿namespace RobotsFactory.WPF
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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string SampleReportsZipFilePath = "../../../../Reports/Sample-Sales-Reports.zip";
        private const string ExtractedReportsPath = @"../../../../Reports/Extracted_Reports";

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
                zipFileProcessor.Extract(SampleReportsZipFilePath, ExtractedReportsPath);

                var matchedDirectories = Utility.GetDirectoriesByPattern(ExtractedReportsPath);
                this.ReadExcelFilesAndCreateSalesReports(matchedDirectories);

                this.ShowMessage("Sales reports from Excel files were successfully imported to MSSQL...");
            }
            catch (Exception)
            {
                this.ShowMessage("Error! Cannot extract and import sales report data from Excel files...");
            }
        }

        private void ExportAggregatedSalesReportToPdfButtonClick(object sender, RoutedEventArgs e)
        {
            this.InitializeDatabaseConnectionIfNecessary();

            try
            {
                var salesReportToPdfFactory = new PdfExportFactoryFromMsSqlDatabase(this.robotsFactoryContext);
                salesReportToPdfFactory.ExportSalesEntriesToPdf();

                this.ShowMessage("Sales Report was successfully exported to PDF...");
            }
            catch (Exception)
            {
                this.ShowMessage("Error! Cannot export sales reports to Pdf file...");
            }
        }

        private void ReadExcelFilesAndCreateSalesReports(IEnumerable<DirectoryInfo> matchedDirectories)
        {
            var zipFileProcessor = new ZipFileProcessor();
            zipFileProcessor.Extract(SampleReportsZipFilePath, ExtractedReportsPath);

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