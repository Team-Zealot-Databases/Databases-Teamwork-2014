namespace RobotsFactory.Excel.Contracts
{
    public interface IExcelSaleReportWriter
    {
        void GenerateExcelReport(string pathToSave, string excelReportName);
    }
}
