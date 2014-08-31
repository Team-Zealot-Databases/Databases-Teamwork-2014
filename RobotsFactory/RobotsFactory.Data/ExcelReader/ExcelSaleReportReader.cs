namespace RobotsFactory.Data.ExcelReader
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.OleDb;
    using System.Linq;
    using RobotsFactory.Data;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.Reports.Models;

    public class ExcelSaleReportReader : IExcelSaleReportReader
    {
        public ExcelReport CreateSaleReport(string dataSourcePath, string reportDateTime)
        {
            var excelReport = new ExcelReport();
            excelReport.ReportDate = DateTime.Parse(reportDateTime);

            using (var excelConnection = new OleDbConnection(this.GetFullConnectionString(dataSourcePath)))
            {
                excelConnection.Open();

                var sheetName = this.GetSheetName(excelConnection);
                var excelCommand = this.GetOleDbCommand(sheetName, excelConnection);

                using (var oleDbDataAdapter = new OleDbDataAdapter(excelCommand))
                {
                    var dataSet = new DataSet();
                    oleDbDataAdapter.Fill(dataSet);

                    using (var reader = dataSet.CreateDataReader())
                    {
                        excelReport.StoreName = this.ReadStoreName(reader);
                        reader.Read(); // Skip column names
                        excelReport.Entries = this.GetReportEntries(reader, excelReport);
                    }
                }
            }

            return excelReport;
        }

        private string ReadStoreName(DataTableReader reader)
        {
            if (reader.Read())
            {
                return reader[0].ToString();
            }

            return null;
        }

        /// <summary>
        /// Gets Excel Report entries and set ReportDate of excelReport object.
        /// </summary>
        private ICollection<ExcelReportEntry> GetReportEntries(DataTableReader reader, ExcelReport excelReport)
        {
            var reportEntries = new List<ExcelReportEntry>();

            while (reader.Read())
            {
                var isLastRow = string.IsNullOrEmpty(reader[1].ToString());

                if (!isLastRow)
                {
                    reportEntries.Add(new ExcelReportEntry()
                    {
                        ProductId = int.Parse(reader[0].ToString()),
                        Quantity = int.Parse(reader[1].ToString()),
                        UnitPrice = decimal.Parse(reader[2].ToString()),
                        Sum = decimal.Parse(reader[3].ToString())
                    });
                }
                else
                {
                    excelReport.TotalSum = decimal.Parse(reader[3].ToString());
                }
            }

            return reportEntries;
        }
 
        private string GetFullConnectionString(string dataSourcePath)
        {
            var fullConnectionString = ConnectionStrings.Default.ExcelConnectionString + "Data Source=" + dataSourcePath;
            return fullConnectionString;
        }
 
        private string GetSheetName(OleDbConnection excelConnection)
        {
            DataTable excelSchema = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string sheetName = excelSchema.Rows[0]["TABLE_NAME"].ToString();
            return sheetName;
        }

        private OleDbCommand GetOleDbCommand(string sheetName, OleDbConnection excelConnection)
        {
            OleDbCommand oleDbCommand = new OleDbCommand(@"SELECT * FROM [" + sheetName + "]", excelConnection);
            return oleDbCommand;
        }
    }
}