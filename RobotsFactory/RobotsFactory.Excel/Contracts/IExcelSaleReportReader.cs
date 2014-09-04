namespace RobotsFactory.Excel.Contracts
{
    using RobotsFactory.Reports.Models;

    public interface IExcelSaleReportReader
    {
        ExcelReport CreateSaleReport(string dataSourcePath, string reportDateTime);
    }
}