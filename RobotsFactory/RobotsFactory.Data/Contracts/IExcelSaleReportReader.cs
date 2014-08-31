namespace RobotsFactory.Data.Contracts
{
    using RobotsFactory.Reports.Models;

    public interface IExcelSaleReportReader
    {
        ExcelReport CreateSaleReport(string dataSourcePath, string reportDateTime);
    }
}