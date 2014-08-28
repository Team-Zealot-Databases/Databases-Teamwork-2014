﻿namespace RobotsFactory.ConsoleClient
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using MongoDB.Driver;
    using RobotsFactory.Data;
    using RobotsFactory.Models;

    public class RobotsFactoryConsoleClient
    {
        private const string SampleReportsPath = "../../../../Reports/Sample-Sales-Reports.zip";
        private const string ExtractedReportsPath = @"../../../../Reports/Extracted_Reports";
        private const string DateTimeNowFormat = "mmssffff";
        private const string DirectorySearchRegexPattern = @"\d{1,2}-\w{2,3}-\d{4}";

        public static void Main()
        {
            ReadDataFromMongoDbCloud();
            //ExtractZipFileAndReadExcelFiles();
            //ConnectAndLoadDataFromMsSql();
        }

        private static void ReadDataFromMongoDbCloud()
        {
            Console.WriteLine("Loading data from MongoDB Cloud Database...\n");

            var mongoClient = new MongoClient("mongodb://zealot:telerik@ds063879.mongolab.com:63879/robotsfactorydata");
            var mongoServer = mongoClient.GetServer();
            var countriesDatabase = mongoServer.GetDatabase("robotsfactorydata");

            var countries = countriesDatabase.GetCollection("Countries");

            foreach (var country in countries.FindAll())
            {
                Console.WriteLine(country["Name"]);
            }
        }

        private static void ExtractZipFileAndReadExcelFiles()
        {
            var currentStamp = DateTime.Now.ToString(DateTimeNowFormat);
            var extractedReportsPathWithStamp = ExtractedReportsPath; // + " - " + currentStamp;

            var zipFileProcessor = new ZipFileProcessor();
            zipFileProcessor.Extract(SampleReportsPath, extractedReportsPathWithStamp);

            var dirInfo = new DirectoryInfo(extractedReportsPathWithStamp);
            var matchedDirectories = dirInfo.GetDirectories().Where(d => Regex.IsMatch(d.Name, DirectorySearchRegexPattern));

            var excelDataReader = new ExcelDataReader();

            foreach (var dir in matchedDirectories)
            {
                foreach (var excelFile in dir.GetFiles("*.xls"))
                {
                    excelDataReader.ReadData(excelFile.FullName);
                }
            }
        }
 
        private static void ConnectAndLoadDataFromMsSql()
        {
            Console.Write("Loading...");

            try
            {
                using (var robotsFactoryContext = new RobotsFactoryContext())
                {
                    robotsFactoryContext.Database.Initialize(true);
                    robotsFactoryContext.Database.CommandTimeout = 5;
                    robotsFactoryContext.Countries.Add(new Country() { Name = "TestCountry" });
                    robotsFactoryContext.SaveChanges();
                    Console.Write("\r");
                    foreach (var country in robotsFactoryContext.Countries.ToList())
                    {
                        Console.WriteLine(country.Name);   
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}