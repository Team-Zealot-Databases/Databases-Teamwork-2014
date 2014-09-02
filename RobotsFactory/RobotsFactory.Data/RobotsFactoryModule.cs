﻿namespace RobotsFactory.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using RobotsFactory.Common;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.Data.ExcelReader;
    using RobotsFactory.Data.PdfProcessor;
    using RobotsFactory.Data.XmlProcessor;
    using RobotsFactory.MongoDb;
    using RobotsFactory.MongoDb.Contracts;
    using RobotsFactory.MongoDb.Mapping;
    using RobotsFactory.Reports.Models;

    public class RobotsFactoryModule
    {
        private readonly IRobotsFactoryData robotsFactoryData = new RobotsFactoryData();
        private readonly IMongoDbContext mongoDbContext = new MongoDbContext();
        private readonly ILogger logger;

        public RobotsFactoryModule(ILogger logger)
        {
            this.logger = logger;
        }

        public void ReadFromMongoDb()
        {
            try
            {
                var mongoDbSeeder = new MongoDbSeeder(this.robotsFactoryData, this.mongoDbContext);
                mongoDbSeeder.Seed();

                this.logger.ShowMessage("Data from MongoDB Cloud Database was successfully seeded in SQL Server...");
            }
            catch (Exception)
            {
                this.logger.ShowMessage("Error! Cannot read data from MongoDb Cloud Database...");
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
            catch (Exception)
            {
                this.logger.ShowMessage("Error! Cannot extract and import sales report data from Excel files...");
            }
        }

        public void ReadAdditionalInformation(string selectedPath)
        {
            try
            {
                var expensesFactory = new ExpensesReportFactoryFromXmlData(this.robotsFactoryData.Manufacturers);
                var xmlReader = new XmlDataReader();
                var xmlData = xmlReader.ReadXmlReportsData(Constants.XmlFilePath);
                this.AddExpensesReportsDataToMSSqlAndMongoDb(xmlData, expensesFactory);

                this.logger.ShowMessage("Additional info from Xml was successfully added to MSSQL Server and MongoDb Cloud Database...");
            }
            catch (Exception)
            {
                this.logger.ShowMessage("Error! Cannot export additional information from XML file to MSSQL Server and MongoDb...");
            }
        }
 
        public void ExportAggregatedSalesReportToPdf(Tuple<string, string> selectedPathAndFileName, DateTime startDate, DateTime endDate)
        {
            try
            {
                var salesReportToPdfFactory = new PdfExportFactoryFromMsSqlDatabase(this.robotsFactoryData);
                salesReportToPdfFactory.ExportSalesEntriesToPdf(selectedPathAndFileName.Item1, selectedPathAndFileName.Item2, startDate, endDate);
                this.logger.ShowMessage("Sales Report was successfully exported to PDF...");
            }
            catch (Exception)
            {
                this.logger.ShowMessage("Error! Cannot export sales reports to Pdf file...");
            }
        }

        public void GenerateXmlReport(Tuple<string, string> selectedPathAndFileName, DateTime startDate, DateTime endDate)
        {
            try
            {
                var xmlGenerator = new XmlReportGenerator(this.robotsFactoryData);
                xmlGenerator.CreateXmlReport(selectedPathAndFileName.Item1, selectedPathAndFileName.Item2, startDate, endDate);

                this.logger.ShowMessage("Sales reports were successfully exported as XML file...");
            }
            catch (Exception)
            {
                this.logger.ShowMessage("Error! Cannot export reports as XML file...");
            }
        }

        private void AddExpensesReportsDataToMSSqlAndMongoDb(IList<XmlVendorExpenseEntry> xmlData, ExpensesReportFactoryFromXmlData expensesFactory)
        {
            foreach (var expenseLog in xmlData)
            {
                var storeExpenseReport = expensesFactory.CreateStoreExpensesReport(expenseLog);
                this.robotsFactoryData.Expenses.Add(storeExpenseReport);
                this.mongoDbContext.ManufacturerExpenses.Insert(new ManufacturerExpenseMap()
                {
                    ManufacturerId = storeExpenseReport.ManufacturerId,
                    ReportDate = storeExpenseReport.ReportDate,
                    Expense = storeExpenseReport.Expense
                });
            }

            this.robotsFactoryData.SaveChanges();
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