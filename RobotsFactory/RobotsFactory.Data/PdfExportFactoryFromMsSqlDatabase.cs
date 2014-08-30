namespace RobotsFactory.Data
{
    using System;
    using System.IO;
    using System.Linq;
    using iTextSharp.text;
    using iTextSharp.text.pdf;

    public class PdfExportFactoryFromMsSqlDatabase
    {
        private readonly RobotsFactoryContext robotsFactoryContext;
        private readonly int tableColumnsNumber = 5;

        public PdfExportFactoryFromMsSqlDatabase(RobotsFactoryContext robotsFactoryContext)
        {
            this.robotsFactoryContext = robotsFactoryContext;
        }

        public void ExportSalesEntriesToPdf()
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

                SetTableTitle(table, cell, boldFont, tableColumnsNumber);
                SetTableColumnHeaders(table, cell, normalFont);

                // execute query for getting all sold items information (product name, quantity, unit price, sum)
                var salesReportEntries =
                    from sre in db.SalesReportEntries
                    join pro in db.Products on sre.ProductId equals pro.ProductId
                    join sl in db.SalesReports on sre.SalesReportEntryId equals sl.SalesReportId
                    select new
                    {
                        Name = pro.Name,
                        Quantity = sre.Quantity,
                        UnitPrice = sre.UnitPrice,
                        Sum = sre.Sum,
                        Date = sl.ReportDate
                    };

                // fill pdf table with the data queried from the database
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
            float[] widths = new float[] { 100f, 90f, 70f, 70f, 70f };
            table.SetWidths(widths);
            return table;
        }

        private static void SetTableTitle(PdfPTable table, PdfPCell cell, Font font, int tableColumnsNumber)
        {
            cell = new PdfPCell(new Phrase("Robots Factory Sales Report", font));
            cell.Colspan = tableColumnsNumber;
            cell.HorizontalAlignment = 1;
            cell.BackgroundColor = new BaseColor(135, 196, 28);
            cell.PaddingTop = 10f;
            cell.PaddingBottom = 10f;
            table.AddCell(cell);
        }

        private void SetTableColumnHeaders(PdfPTable table, PdfPCell cell, Font font)
        {
            cell = new PdfPCell(new Phrase("Product Name", font));
            cell.Colspan = 1;
            cell.HorizontalAlignment = 1;
            cell.BackgroundColor = new BaseColor(135, 196, 28);
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Quantity", font));
            cell.Colspan = 1;
            cell.BackgroundColor = new BaseColor(135, 196, 28);
            cell.HorizontalAlignment = 1;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Unit Price", font));
            cell.Colspan = 1;
            cell.BackgroundColor = new BaseColor(135, 196, 28);
            cell.HorizontalAlignment = 1;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Sum", font));
            cell.Colspan = 1;
            cell.BackgroundColor = new BaseColor(135, 196, 28);
            cell.HorizontalAlignment = 1;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date", font));
            cell.Colspan = 1;
            cell.BackgroundColor = new BaseColor(135, 196, 28);
            cell.HorizontalAlignment = 1;
            cell.PaddingBottom = 5f;
            cell.PaddingLeft = 5f;
            table.AddCell(cell);
        }

    }
}