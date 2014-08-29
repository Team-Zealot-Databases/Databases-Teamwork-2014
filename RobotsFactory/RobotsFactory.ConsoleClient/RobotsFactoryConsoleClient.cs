namespace RobotsFactory.ConsoleClient
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using RobotsFactory.Common;
    using RobotsFactory.Data;
    using RobotsFactory.Data.ExcelProcessor;

    public class RobotsFactoryConsoleClient
    {
        private const string SampleReportsPath = "../../../../Reports/Sales-Reports.zip";
        private const string ExtractedReportsPath = @"../../../../Reports/Extracted_Reports";
        private const string DateTimeNowFormat = "mmssffff";
        private const string DirectorySearchRegexPattern = @"\d{1,2}-\w{2,3}-\d{4}";

        public static void Main()
        {
            //var mongoDatabase = new MongoDbCloudDatabase();
            //mongoDatabase.PrintCollectionItems("Countries");
            //mongoDatabase.PrintCollectionItems("ProductTypes");

            //ExtractZipFileAndReadExcelFiles();
            ConnectAndLoadDataFromMsSql();
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
            Console.WriteLine("Loading data from MongoDB Cloud Database and seed it in SQL Server...\n");

            try
            {
                using (var robotsFactoryContext = new RobotsFactoryContext())
                {
                    robotsFactoryContext.Database.Initialize(true);
                    PrintCountries(robotsFactoryContext);
                    PrintCities(robotsFactoryContext);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
 
        private static void PrintCountries(RobotsFactoryContext robotsFactoryContext)
        {
            Console.WriteLine("--------- Countries (from SQL Server): ");

            foreach (var country in robotsFactoryContext.Countries.ToList())
            {
                Console.WriteLine(country.Name);   
            }

            Console.WriteLine();
        }

        private static void PrintCities(RobotsFactoryContext robotsFactoryContext)
        {
            Console.WriteLine("--------- Cities (from SQL Server): ");

            foreach (var city in robotsFactoryContext.Cities.ToList())
            {
                Console.WriteLine(city.Name);
            }

            Console.WriteLine();
        }
    }
}