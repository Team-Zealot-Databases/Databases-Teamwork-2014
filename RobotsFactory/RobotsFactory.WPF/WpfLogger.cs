namespace RobotsFactory.WPF
{
    using System;
    using System.Linq;
    using System.Windows.Controls;
    using RobotsFactory.Data.Contracts;

    public class WpfLogger : ILogger
    {
        private readonly TextBlock textBlock;

        public WpfLogger(TextBlock textBlock)
        {
            this.textBlock = textBlock;
        }

        public void ShowMessage(string message)
        {
            this.textBlock.Text = message;
        }
    }
}