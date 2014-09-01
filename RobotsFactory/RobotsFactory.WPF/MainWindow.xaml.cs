namespace RobotsFactory.WPF
{
    using System;
    using System.Linq;
    using System.Windows;
    using Microsoft.Win32;
    using RobotsFactory.Common;
    using RobotsFactory.Data;
    using RobotsFactory.Data.Contracts;
    using RobotsFactory.WPF.Contracts;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IRobotsFactoryData robotsFactoryData = new RobotsFactoryData();
        private RobotsFactoryModule robotsFactoryModule;
        private ILogger logger;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void OnWindowFormLoaded(object sender, RoutedEventArgs e)
        {
            this.logger = new WpfLogger(this.messageBox);
            this.robotsFactoryModule = new RobotsFactoryModule(this.logger);
        }

        private void OnReadFromMongoDbButtonClick(object sender, RoutedEventArgs e)
        {
            this.robotsFactoryModule.ReadFromMongoDb();
        }

        private void OnReadSaleReportsFromExcelButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedPath = this.OpenFileDialogBox("zip");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                this.robotsFactoryModule.ReadSaleReportsFromExcel(selectedPath);
            }
        }

        private void OnExportAggregatedSalesReportToPdfButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedPathAndFileName = this.SaveFileDialogBox("pdf", "Robots-Factory-Aggrerated-Sales-Report.pdf");
            if (selectedPathAndFileName != null)
            {
                var startDate = Utility.GetSelectedDateOrDefault(this.startDateTimePicker.Text, DateTime.MinValue);
                var endDate = Utility.GetSelectedDateOrDefault(this.endDateTimePicker.Text, DateTime.Now);

                this.robotsFactoryModule.ExportAggregatedSalesReportToPdf(selectedPathAndFileName, startDate, endDate);
            }
        }

        private void OnGenerateXmlReportButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedPathAndFileName = this.SaveFileDialogBox("xml", Constants.XmlReportName);
            if (selectedPathAndFileName != null)
            {
                var startDate = Utility.GetSelectedDateOrDefault(this.startDateTimePicker.Text, DateTime.MinValue);
                var endDate = Utility.GetSelectedDateOrDefault(this.endDateTimePicker.Text, DateTime.Now);

                this.robotsFactoryModule.GenerateXmlReport(selectedPathAndFileName, startDate, endDate);
            }
        }

        private void OnReadAdditionalInformationFromXmlButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedPath = this.OpenFileDialogBox("xml");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                this.robotsFactoryModule.ReadAdditionalInformation(selectedPath);
                return;
            }
        }

        private void OnOpenReportsDirectoryButtonClick(object sender, RoutedEventArgs e)
        {
            Utility.OpenDirectory(Constants.ReportsDirectoryPath);
        }
 
        private string OpenFileDialogBox(string typeOfFileToRead)
        {
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = Constants.ReportsDirectoryPath;
            Utility.SetFileDialogFilter(dialog, typeOfFileToRead);

            var result = dialog.ShowDialog();
            if (result == true)
            {
                return dialog.FileName;
            }

            return string.Empty;
        }

        private Tuple<string, string> SaveFileDialogBox(string typeOfFileToSave, string defaultFileName)
        {
            var dialog = new SaveFileDialog();
            dialog.InitialDirectory = Constants.ReportsDirectoryPath;
            Utility.SetFileDialogFilter(dialog, typeOfFileToSave, defaultFileName);

            var result = dialog.ShowDialog();
            if (result == true)
            {
                return new Tuple<string, string>(dialog.FileName, dialog.SafeFileName);
            }

            return null;
        }
    }
}