namespace RobotsFactory.Json
{
    using System;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;
    using RobotsFactory.Common;
    using RobotsFactory.MySQL;
    using RobotsFactory.Reports.Models;

    public class JsonReportsFactoryFromMsSqlDatabase
    {
        private const string JsonFormat = ".json";

        private readonly IQueryable<JsonProductsReportEntry> productsReportsEntries;
        private readonly RobotsFactoryMySqlContext robotsFactoryMySqlContext = new RobotsFactoryMySqlContext();

        public JsonReportsFactoryFromMsSqlDatabase(IQueryable<JsonProductsReportEntry> productsReportsEntries)
        {
            this.productsReportsEntries = productsReportsEntries;
        }

        public void SaveJsonProductsReportsToDisk(string pathToSave)
        {
            Utility.CreateDirectoryIfNotExists(pathToSave);

            foreach (JsonProductsReportEntry productReport in this.productsReportsEntries)
            {
                string fileName = (productReport.ProductId + JsonFormat).ToString();
                string singleProductReport = this.CreateJsonReportForSinleProduct(productReport);

                using (StreamWriter jsonReportFile = new StreamWriter(string.Format("{0}\\{1}", pathToSave, fileName), true))
                {
                    jsonReportFile.Write(singleProductReport);
                }
            }
        }

        public void ExportJsonProductsReportsToMySqlDatabase()
        {
            foreach (JsonProductsReportEntry productReport in this.productsReportsEntries)
            {
                string singleProductReport = this.CreateJsonReportForSinleProduct(productReport);

                this.robotsFactoryMySqlContext.Add(new JsonReport()
                {
                    JsonContent = singleProductReport,
                });
            }

            this.robotsFactoryMySqlContext.SaveChanges();
        }

        private string CreateJsonReportForSinleProduct(JsonProductsReportEntry singleProduct)
        {
            var serializedProductReportEntry = JsonConvert.SerializeObject(singleProduct);
            return serializedProductReportEntry;
        }
    }
}