namespace RobotsFactory.Data.PdfProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using RobotsFactory.Common;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.Reports.Models;
    using iTextSharp.text;
    using iTextSharp.text.pdf;

    public class PdfExportFactoryFromMsSqlDatabase
    {
        private const string DateTimeFormat = "dd.MM.yyyy";
        private const int TableColumnsNumber = 5;

        private readonly IRobotsFactoryData robotsFactoryData;

        public PdfExportFactoryFromMsSqlDatabase(IRobotsFactoryData robotsFactoryData)
        {
            this.robotsFactoryData = robotsFactoryData;
        }

        public void ExportSalesEntriesToPdf(string pathToSave, string pdfReportName, DateTime startDate, DateTime endDate)
        {
            if (!string.IsNullOrEmpty(pdfReportName))
            {
                Utility.CreateDirectoryIfNotExists(pathToSave);
            }

            using (var doc = this.InitializePdfDocument(pathToSave + pdfReportName))
            {
                doc.Open();
                var table = this.InitializePdfTable(TableColumnsNumber);

                // Set fonts
                var bfTimes = BaseFont.CreateFont(Constants.OpenSansRecularTtfPath, BaseFont.CP1252, false);
                var normalFont = new Font(bfTimes, 10);
                var boldFont = new Font(bfTimes, 11, Font.BOLD);

                this.SetTableTitle(table, boldFont, TableColumnsNumber, startDate, endDate);
                //this.SetTableColumnHeaders(table, normalFont);

                var salesReportEntries = this.GetSaleReportsFromDatabase(startDate, endDate);
                this.FillPdfTableBody(salesReportEntries, table, normalFont);

                decimal totalSum = salesReportEntries.Sum(x => x.Sum);
                this.SetTableFooter(table, totalSum, boldFont, TableColumnsNumber);

                doc.Add(table);
            }
        }

        /// <summary>
        // Query for getting all sold items information (product name, quantity, unit price, sum, date)
        /// </summary>
        private IQueryable<PdfSaleReportEntry> GetSaleReportsFromDatabase(DateTime startDate, DateTime endDate)
        {
            var salesReportEntries = from sre in this.robotsFactoryData.SalesReportEntries.All()
                                     join pro in this.robotsFactoryData.Products.All() on sre.ProductId equals pro.ProductId
                                     join sl in this.robotsFactoryData.SalesReports.All() on sre.SalesReportId equals sl.SalesReportId
                                     where sl.ReportDate >= startDate && sl.ReportDate <= endDate
                                     orderby sl.ReportDate
                                     select new PdfSaleReportEntry
                                     {
                                         Name = pro.Name,
                                         Quantity = sre.Quantity,
                                         Date = sl.ReportDate,
                                         UnitPrice = sre.UnitPrice,
                                         Location = sl.Store.Name,
                                         Sum = sre.Sum
                                     };

            return salesReportEntries;
        }

        /// <summary>
        // Fill pdf table body with the data queried from the database
        /// </summary>
        private void FillPdfTableBody(IQueryable<PdfSaleReportEntry> salesReportEntries, PdfPTable table, Font normalFont)
        {
            DateTime currentDate = DateTime.MinValue;

            foreach (var salesEntry in salesReportEntries)
            {
                if (currentDate != salesEntry.Date)
                {
                    this.AddDateCell(table, normalFont, TableColumnsNumber, new BaseColor(189, 215, 238), salesEntry.Date);
                    this.SetTableColumnHeaders(table, normalFont);
                }
                currentDate = salesEntry.Date;

                this.AddTableCell(table, normalFont, null, salesEntry.Name);
                this.AddTableCell(table, normalFont, null, salesEntry.Quantity.ToString());
                this.AddTableCell(table, normalFont, null, salesEntry.UnitPrice.ToString());
                this.AddTableCell(table, normalFont, null, salesEntry.Location);
                this.AddTableCell(table, normalFont, null, salesEntry.Sum.ToString());
            }
        }

        private Document InitializePdfDocument(string pdfFullPath)
        {
            var doc = new Document();
            var file = File.Create(pdfFullPath);
            PdfWriter.GetInstance(doc, file);
            return doc;
        }

        private PdfPTable InitializePdfTable(int columnsNumber)
        {
            var table = new PdfPTable(columnsNumber);
            table.TotalWidth = 500f;
            table.LockedWidth = true;
            float[] widths = new float[] { 120f, 120, 120f, 120f, 120f };
            table.SetWidths(widths);
            return table;
        }

        private void SetTableTitle(PdfPTable table, Font font, int tableColumnsNumber, DateTime startDate, DateTime endDate)
        {
            var cell = new PdfPCell(new Phrase(string.Format("Robots Factory - Sales Report ({0} - {1})",
                startDate.ToString(DateTimeFormat), endDate.ToString(DateTimeFormat)), font));

            cell.Colspan = tableColumnsNumber;
            cell.HorizontalAlignment = 1;
            cell.BackgroundColor = new BaseColor(189, 215, 238);
            cell.PaddingTop = 10f;
            cell.PaddingBottom = 10f;
            table.AddCell(cell);
        }

        private void SetTableFooter(PdfPTable table, decimal totalSum, Font font, int tableColumnsNumber)
        {
            var cell = new PdfPCell(new Phrase("Total Sum: $ " + totalSum.ToString(), font));
            cell.Colspan = tableColumnsNumber - 1;
            cell.HorizontalAlignment = 2;
            cell.BackgroundColor = new BaseColor(150, 206, 163);
            cell.PaddingBottom = 5f;
            cell.PaddingRight = 35f;
            cell.BorderWidthRight = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(""));
            cell.Colspan = 1;
            cell.BackgroundColor = new BaseColor(150, 206, 163);
            cell.BorderWidthLeft = 0;
            table.AddCell(cell);
        }

        private void SetTableColumnHeaders(PdfPTable table, Font font)
        {
            var baseColor = new BaseColor(221, 235, 247);

            this.AddTableCell(table, font, baseColor, "Product");
            this.AddTableCell(table, font, baseColor, "Quantity");
            this.AddTableCell(table, font, baseColor, "Unit Price");
            this.AddTableCell(table, font, baseColor, "Location");
            this.AddTableCell(table, font, baseColor, "Sum");
        }

        private void AddTableCell(PdfPTable table, Font font, BaseColor baseColor, string phraseName)
        {
            var cell = new PdfPCell(new Phrase(phraseName, font));
            cell.Colspan = 1;
            cell.HorizontalAlignment = 1;
            cell.BackgroundColor = baseColor;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            table.AddCell(cell);
        }

        private void AddDateCell(PdfPTable table, Font font, int tableColumnsNumber, BaseColor backgroundColor, DateTime date)
        {
            var cell = new PdfPCell(new Phrase(string.Format("Date: {0}",
                date.ToString(DateTimeFormat)), font));

            cell.Colspan = tableColumnsNumber;
            cell.HorizontalAlignment = 0;
            cell.BackgroundColor = backgroundColor;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            table.AddCell(cell);
        }
    }
}