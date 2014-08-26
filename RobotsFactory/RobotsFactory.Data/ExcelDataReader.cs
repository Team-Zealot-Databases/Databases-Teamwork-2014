using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace RobotsFactory.Data
{
    public class ExcelDataReader
    {
        public void ReadData(string dataSourcePath)
        {
            var dataSourceFormat = "Data Source=" + dataSourcePath;
            var excelConnection = new OleDbConnection(ConnectionStrings.Default.ExcelConnectionString + dataSourceFormat);
            excelConnection.Open();

            DataTable excelSchema = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string sheetName = excelSchema.Rows[0]["TABLE_NAME"].ToString();

            OleDbCommand excelCommand = new OleDbCommand(@"SELECT * FROM [" + sheetName + "]", excelConnection);

            var data = new List<string>();

            using (excelConnection)
            {
                using (OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(excelCommand))
                {
                    DataSet dataSet = new DataSet();
                    oleDbDataAdapter.Fill(dataSet);

                    using (DataTableReader reader = dataSet.CreateDataReader())
                    {
                        if (reader.Read())
                        {
                            data.Add(reader[0].ToString()); // Factory name
                        }

                        reader.Read(); // Skip column names

                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (!string.IsNullOrEmpty(reader[i].ToString()))
                                {
                                    data.Add(reader[i].ToString());
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine(string.Join(" ", data));
        }
    }
}