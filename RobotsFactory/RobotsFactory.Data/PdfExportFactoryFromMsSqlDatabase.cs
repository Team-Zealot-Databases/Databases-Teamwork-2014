namespace RobotsFactory.Data
{
    using System;
    using System.IO;
    using System.Linq;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using System.Globalization;

    public class PdfExportFactoryFromMsSqlDatabase
    {
        private readonly RobotsFactoryContext robotsFactoryContext;
        private readonly int tableColumnsNumber = 5;

        public PdfExportFactoryFromMsSqlDatabase(RobotsFactoryContext robotsFactoryContext)
        {
            this.robotsFactoryContext = robotsFactoryContext;
        }

        public void ExportSalesEntriesToPdf(string startDateString, string endDateString)
        {
            using (var db = this.robotsFactoryContext)
            {
                Document doc = InitializePdfDocument();
                PdfPTable table = InitializePdfTable(tableColumnsNumber);
                PdfPCell cell = new PdfPCell();

                // set fonts
                BaseFont bfTimes = BaseFont.CreateFont("../../../RobotsFactory.Data/OpenSans-Regular.ttf", BaseFont.CP1252, false);
                Font normalFont = new Font(bfTimes, 10);
                Font boldFont = new Font(bfTimes, 11, Font.BOLD);

                SetTableTitle(table, cell, boldFont, tableColumnsNumber, startDateString, endDateString);
                SetTableColumnHeaders(table, cell, normalFont);

                DateTime startDate = DateTime.ParseExact(startDateString, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(endDateString, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                // query for getting all sold items information (product name, quantity, unit price, sum, date)
                var salesReportEntries =
                    from sre in db.SalesReportEntries
                    join pro in db.Products on sre.ProductId equals pro.ProductId
                    join sl in db.SalesReports on sre.SalesReportEntryId equals sl.SalesReportId
                    where sl.ReportDate >= startDate && sl.ReportDate <= endDate
                    orderby sl.ReportDate
                    select new
                    {
                        Name = pro.Name,
                        Quantity = sre.Quantity,
                        UnitPrice = sre.UnitPrice,
                        Sum = sre.Sum,
                        Date = sl.ReportDate
                    };

                // fill pdf table body with the data queried from the database
                foreach (var salesEntry in salesReportEntries)
                {
                    cell = new PdfPCell(new Phrase(salesEntry.Name, normalFont));
                    cell.Colspan = 1;
                    cell.HorizontalAlignment = 1;
                    cell.PaddingBottom = 5f;
                    cell.PaddingLeft = 5f;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(salesEntry.Quantity.ToString(), normalFont));
                    cell.Colspan = 1;
                    cell.HorizontalAlignment = 1;
                    cell.PaddingBottom = 5f;
                    cell.PaddingLeft = 5f;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(salesEntry.UnitPrice.ToString(), normalFont));
                    cell.Colspan = 1;
                    cell.HorizontalAlignment = 1;
                    cell.PaddingBottom = 5f;
                    cell.PaddingLeft = 5f;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(salesEntry.Sum.ToString(), normalFont));
                    cell.Colspan = 1;
                    cell.HorizontalAlignment = 1;
                    cell.PaddingBottom = 5f;
                    cell.PaddingLeft = 5f;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(salesEntry.Date.ToString("dd.MM.yyyy"), normalFont));
                    cell.Colspan = 1;
                    cell.HorizontalAlignment = 1;
                    cell.PaddingBottom = 5f;
                    cell.PaddingLeft = 5f;
                    table.AddCell(cell);
                }

                decimal totalSum = salesReportEntries.Sum(x => x.Sum);
                SetTableFooter(table, cell, totalSum, boldFont, tableColumnsNumber);

                doc.Add(table);
                doc.Close();
            }
        }

        private static Document InitializePdfDocument()
        {
            Document doc = new Document();
            FileStream file = File.Create("../../../../Reports/Robots-Factory-Aggrerated-Sales-Report.pdf");
            PdfWriter.GetInstance(doc, file);
            doc.Open();
            return doc;
        }

        private static PdfPTable InitializePdfTable(int columnsNumber)
        {
            PdfPTable table = new PdfPTable(columnsNumber);
            table.TotalWidth = 500f;
            table.LockedWidth = true;
            float[] widths = new float[] { 100f, 100, 100f, 100f, 100f };
            table.SetWidths(widths);
            return table;
        }

        private static void SetTableTitle(PdfPTable table, PdfPCell cell, Font font, int tableColumnsNumber, string startDateString,
            string endDateString)
        {
            cell = new PdfPCell(new Phrase(string.Format("Robots Factory - Sales Report ({0} - {1})",
                startDateString, endDateString), font));
            cell.Colspan = tableColumnsNumber;
            cell.HorizontalAlignment = 1;
            cell.BackgroundColor = new BaseColor(189, 215, 238);
            cell.PaddingTop = 10f;
            cell.PaddingBottom = 10f;
            table.AddCell(cell);
        }

        private void SetTableColumnHeaders(PdfPTable table, PdfPCell cell, Font font)
        {
            cell = new PdfPCell(new Phrase("Product Name", font));
            cell.Colspan = 1;
            cell.HorizontalAlignment = 1;
            cell.BackgroundColor = new BaseColor(221, 235, 247);
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Quantity", font));
            cell.Colspan = 1;
            cell.BackgroundColor = new BaseColor(221, 235, 247);
            cell.HorizontalAlignment = 1;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Unit Price", font));
            cell.Colspan = 1;
            cell.BackgroundColor = new BaseColor(221, 235, 247);
            cell.HorizontalAlignment = 1;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Sum", font));
            cell.Colspan = 1;
            cell.BackgroundColor = new BaseColor(221, 235, 247);
            cell.HorizontalAlignment = 1;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date", font));
            cell.Colspan = 1;
            cell.BackgroundColor = new BaseColor(221, 235, 247);
            cell.HorizontalAlignment = 1;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            table.AddCell(cell);
        }

        private static void SetTableFooter(PdfPTable table, PdfPCell cell, decimal totalSum, Font font, int tableColumnsNumber)
        {
            cell = new PdfPCell(new Phrase("Total Sum: $ " + totalSum.ToString(), font));
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
    }
}