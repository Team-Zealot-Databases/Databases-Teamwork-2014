namespace RobotsFactory.Common
{
    using System;
    using System.IO;
    using System.Linq;

    public static class Constants
    {
        public const string SampleReportsZipFilePath = "../../../../Reports/Sales-Reports.zip";
        public const string AggregatedSaleReportPdfPath = "../../../../Reports/PDF_Reports";
        public const string ExtractedXmlReportsPath = @"../../../../Reports/XML_Reports/";
        public const string ExtractedExcelReportsPath = @"../../../../Reports/Excel_Reports";
        public const string XmlFilePath = "../../../../Reports/Vendors-Expenses.xml";
        public const string XmlFilePath2 = "../../../../Reports/Vendors-Expenses-2.xml";
        public const string XmlReportName = @"xml-report.xml";
        public const string PdfReportName = "/Robots-Factory-Aggrerated-Sales-Report.pdf";
        public const string ExcelFileExtensionPattern = "*.xls";
        public const string OpenSansRecularTtfPath = "../../../RobotsFactory.Data/OpenSans-Regular.ttf";

        public static string ReportsDirectoryPath
        {
            get
            {
                try 
                { 
                    return	Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName + "\\Reports";
                }
                catch (Exception)
                {
                    return Directory.GetCurrentDirectory();
                }
            }
        }
    }
}