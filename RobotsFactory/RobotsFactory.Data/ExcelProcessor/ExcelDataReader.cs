namespace RobotsFactory.Data.ExcelProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.OleDb;
    using System.Linq;
    using RobotsFactory.Data;

    public class ExcelDataReader
    {
        public IList<string> ReadSaleReportsData(string dataSourcePath)
        {
            var dataSourceFormat = "Data Source=" + dataSourcePath;
            var dataRowsAsStrings = new List<string>();

            using (var excelConnection = new OleDbConnection(ConnectionStrings.Default.ExcelConnectionString + dataSourceFormat))
            {
                excelConnection.Open();

                string sheetName = this.GetSheetName(excelConnection);
                var excelCommand = this.GetOleDbCommand(sheetName, excelConnection);

                using (OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(excelCommand))
                {
                    DataSet dataSet = new DataSet();
                    oleDbDataAdapter.Fill(dataSet);

                    using (DataTableReader reader = dataSet.CreateDataReader())
                    {
                        if (reader.Read())
                        {
                            dataRowsAsStrings.Add(reader[0].ToString()); // Factory name
                        }

                        reader.Read(); // Skip column names

                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (!string.IsNullOrEmpty(reader[i].ToString()))
                                {
                                    dataRowsAsStrings.Add(reader[i].ToString());
                                }
                            }
                        }
                    }
                }
            }

            return dataRowsAsStrings;
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