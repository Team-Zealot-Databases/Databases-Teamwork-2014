﻿namespace RobotsFactory.ConsoleClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using RobotsFactory.Common;
    using RobotsFactory.Data;
    using RobotsFactory.Data.ExcelProcessor;

    public class RobotsFactoryConsoleClient
    {
        private const string SampleReportsZipFilePath = "../../../../Reports/Sample-Sales-Reports.zip";
        private const string ExtractedReportsPath = @"../../../../Reports/Extracted_Reports";

        public static void Main()
        {
            ConnectAndLoadDataFromMsSql();
        }

        private static void ConnectAndLoadDataFromMsSql()
        {
            try
            {
                using (var robotsFactoryContext = new RobotsFactoryContext())
                {
                    robotsFactoryContext.Database.Initialize(true);
                    SeedDataFromMongoDB(robotsFactoryContext);
                    ExtractZipAndReadSalesReportExcelFiles(robotsFactoryContext);
                }
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
            zipFileProcessor.Extract(SampleReportsZipFilePath, ExtractedReportsPath);

            var matchedDirectories = Utility.GetDirectoriesByPattern(ExtractedReportsPath);
            ReadExcelFilesAndCreateSalesReports(robotsFactoryContext, matchedDirectories);
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
    }
}