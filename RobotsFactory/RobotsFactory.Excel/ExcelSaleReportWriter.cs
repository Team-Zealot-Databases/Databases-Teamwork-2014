﻿namespace RobotsFactory.Excel
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Drawing;

    using OfficeOpenXml;
    using Newtonsoft.Json;
    using OfficeOpenXml.Style;

    using RobotsFactory.MySQL;
    using RobotsFactory.SQLite;
    using RobotsFactory.Common;
    using RobotsFactory.Reports.Models;
    using RobotsFactory.Excel.Contracts;

    public class ExcelSaleReportWriter : IExcelSaleReportWriter
    {
        private const int ProductNameColumn = 2;
        private const int QuantityColumn = 3;
        private const int TotalIncomeColumn = 4;
        private const int ExpensePerItemColumn = 5;
        private const int TotalExpensesColumn = 6;
        private const int RevenueColumn = 7;

        private int bodyRowPosition = 3;

        private RobotsFactoryMySqlContext robotFactoryMysql;
        private SQLiteDbContext robotsFactorySqlite;

        public ExcelSaleReportWriter()
        {
            this.robotFactoryMysql = new RobotsFactoryMySqlContext();
            this.robotsFactorySqlite = new SQLiteDbContext();
        }

        public void GenerateExcelReport(string path)
        {
            var fileName = "/Report" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + ".xlsx";

            Utility.CreateDirectoryIfNotExists(path);

            var filePath = new FileInfo(path + fileName);

            using (var package = new ExcelPackage(filePath))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Net Revenue");

                GenerateExcelHeadDocument(worksheet);

                GenerateExcelBodyDocument(worksheet);

                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();

                package.Save();
            }
        }

        private void GenerateExcelBodyDocument(ExcelWorksheet worksheet)
        {
            foreach (var jsonReport in this.robotFactoryMysql.JsonReports)
            {
                var currentEntity = JsonConvert.DeserializeObject<JsonProductsReportEntry>(jsonReport.JsonContent);
                this.bodyRowPosition++;
                FillCurrentRowWithData(worksheet, currentEntity);
                StyleCurrentExcelRow(this.bodyRowPosition, worksheet);
            }
        }

        private void FillCurrentRowWithData(ExcelWorksheet worksheet, JsonProductsReportEntry currentEntity)
        {
            var productName = (string)currentEntity.ProductName;
            var quantity = currentEntity.TotalQuantitySold;
            var totalIncome = (decimal)currentEntity.TotalIncome;
            var expensePerItem = GetItemExpense(currentEntity);
            var totalExpenses = quantity * expensePerItem;
            var revenue = totalIncome - totalExpenses;

            worksheet.Cells[this.bodyRowPosition, ProductNameColumn].Value = productName;
            worksheet.Cells[this.bodyRowPosition, QuantityColumn].Value = quantity;
            worksheet.Cells[this.bodyRowPosition, TotalIncomeColumn].Value = totalIncome;
            worksheet.Cells[this.bodyRowPosition, ExpensePerItemColumn].Value = expensePerItem;
            worksheet.Cells[this.bodyRowPosition, TotalExpensesColumn].Value = totalExpenses;
            worksheet.Cells[this.bodyRowPosition, RevenueColumn].Value = revenue;
        }

        private decimal GetItemExpense(JsonProductsReportEntry currentEntity)
        {
            var itemExpense = this.robotsFactorySqlite.Items.Where(i => i.Name == currentEntity.ProductName)
                                                            .Select(i => new
                                                            {
                                                                Expense = i.Expense
                                                            })
                                                            .First();

            return (decimal)itemExpense.Expense;
        }

        private void StyleCurrentExcelRow(int currentRow, ExcelWorksheet worksheet)
        {
            using (var rowRange = worksheet.Cells[currentRow, 2, currentRow, 7])
            {
                rowRange.Style.Font.Bold = false;
                rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rowRange.Style.Fill.BackgroundColor.SetColor(Color.Azure);
                rowRange.Style.Font.Color.SetColor(Color.Black);
                rowRange.Style.ShrinkToFit = false;
                rowRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                rowRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                rowRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                rowRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

        }

        private void GenerateExcelHeadDocument(ExcelWorksheet worksheet)
        {
            // head document styling
            worksheet.Row(2).Height = 20;
            worksheet.Row(3).Height = 18;

            worksheet.Cells[2, 2].Value = "Report For Product Net Revenue";
            var headerRange = worksheet.Cells[2, 2, 2, 7];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.Size = 16;
            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

            worksheet.Cells[3, ProductNameColumn].Value = "Product Name";
            worksheet.Cells[3, QuantityColumn].Value = "Total Quantity Sold";
            worksheet.Cells[3, TotalIncomeColumn].Value = "Total Income";
            worksheet.Cells[3, ExpensePerItemColumn].Value = "Expenses Per Item";
            worksheet.Cells[3, TotalExpensesColumn].Value = "Total Expenses";
            worksheet.Cells[3, RevenueColumn].Value = "Net Revenue";

            using (var range = worksheet.Cells[3, 2, 3, 7])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.Black);
                range.Style.Font.Color.SetColor(Color.WhiteSmoke);
                range.Style.ShrinkToFit = false;
            }
        }
    }
}
