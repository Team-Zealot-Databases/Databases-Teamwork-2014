namespace RobotsFactory.Data.Contracts
{
    using System;
    using System.Linq;

    public interface ILogger
    {
        void ShowMessage(string message);
    }
}