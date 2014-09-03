namespace RobotsFactory.Json
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using RobotsFactory.Common;
    using RobotsFactory.MySQL;
    using RobotsFactory.Reports.Models;

    public class JsonReportsFactoryFromMsSqlDatabase
    {
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
                string fileName = (productReport.ProductId + ".json").ToString();
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
            StringBuilder sb = new StringBuilder();
            string indentation = new String(' ', Constants.IndentSymbolsNumber);
            sb.Append("{").AppendLine();
            sb.Append(indentation).AppendFormat("\"product-id\": {0},", singleProduct.ProductId).AppendLine();
            sb.Append(indentation).AppendFormat("\"product-name\": \"{0}\",", singleProduct.ProductName).AppendLine();
            sb.Append(indentation).AppendFormat("\"total-quantity-sold\": {0},", singleProduct.TotalQuantitySold).AppendLine();
            sb.Append(indentation).AppendFormat("\"total-income\": {0}", singleProduct.TotalIncome).AppendLine();
            sb.Append("}");

            string singleProductJson = sb.ToString();
            return singleProductJson;
        }
    }
}