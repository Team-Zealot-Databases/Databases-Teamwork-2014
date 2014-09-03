namespace RobotsFactory.Data.JsonProcessor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using MySql.Data.MySqlClient;
    using RobotsFactory.Common;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.Reports.Models;
    using RobotsFactory.MySQL;

    public class JsonReportsFactoryFromMsSqlDatabase
    {
        private readonly IRobotsFactoryData robotsFactoryData;
        private IQueryable<JsonProductsReportEntry> productsReportsEntries;
        private readonly RobotsFactoryMySqlContext robotsFactoryMySqlContext = new RobotsFactoryMySqlContext();

        public JsonReportsFactoryFromMsSqlDatabase(IRobotsFactoryData robotsFactoryData)
        {
            this.robotsFactoryData = robotsFactoryData;
            this.productsReportsEntries = GetProductsReportsFromDatabase();
        }

        public void SaveJsonProductsReportsToDisk(string pathToSave)
        {
            Utility.CreateDirectoryIfNotExists(pathToSave);
            foreach (JsonProductsReportEntry productReport in this.productsReportsEntries)
            {
                string fileName = (productReport.ProductId + ".json").ToString();
                string singleProductReport = CreateJsonReportForSinleProduct(productReport);

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
                string singleProductReport = CreateJsonReportForSinleProduct(productReport);
                robotsFactoryMySqlContext.Add(new JsonReport()
                {
                    JsonContent = singleProductReport,
                });
            }
            robotsFactoryMySqlContext.SaveChanges();
        }

        private IQueryable<JsonProductsReportEntry> GetProductsReportsFromDatabase()
        {
            var productsReportsEntries = from pro in this.robotsFactoryData.Products.All()
                                         join man in this.robotsFactoryData.Manufacturers.All() on pro.ManufacturerId equals man.ManufacturerId
                                         join sre in this.robotsFactoryData.SalesReportEntries.All() on pro.ProductId equals sre.ProductId
                                         group sre by new { ProductId = sre.ProductId, ProductName = pro.Name, ManufacturerName = man.Name } into grp
                                         select new JsonProductsReportEntry()
                                         {
                                             ProductId = grp.Key.ProductId,
                                             ProductName = grp.Key.ProductName,
                                             ManufacturerName = grp.Key.ManufacturerName,
                                             TotalQuantitySold = grp.Sum(x => x.Quantity),
                                             TotalIncome = grp.Sum(x => x.Sum)
                                         };


            return productsReportsEntries;
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

        private void CreateMySqlDatabase(string databaseName)
        {
            string connectionString = "server=localhost;user=root;port=3306;password=erase65o;";
            //string connectionString = "server=db4free.net;user=zealot;port=3306;password=telerik;database=robotsfactory";
            using (var conn = new MySqlConnection(connectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = string.Format("CREATE DATABASE IF NOT EXISTS `{0}`;", databaseName);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = string.Format("USE {0};", databaseName);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE JsonProductsReports (" +
                                            "product_report_id INT NOT NULL AUTO_INCREMENT," +
                                            "json_product_report VARCHAR(255) NOT NULL," +
                                            "PRIMARY KEY (product_report_id)" +
                                        ");";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void CreateMySqlDatabaseTable(string tableName)
        {
        }
    }
}