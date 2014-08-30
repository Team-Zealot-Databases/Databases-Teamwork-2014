namespace RobotsFactory.Data.XmlProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using RobotsFactory.Models;

    public class XmlDataReader
    {
        public IList<XmlVendorExpenseEntry> ReadXmlReportsData(string dataSourcePath)
        {
            var xmlReader = new XmlTextReader(dataSourcePath);
            var xmlData = new List<XmlVendorExpenseEntry>();
            var storeName = string.Empty;
            var date = new DateTime();

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    while (xmlReader.MoveToNextAttribute())
                    {
                        if (xmlReader.Name == "name")
                        {
                            storeName = xmlReader.Value;
                        }
                        else if (xmlReader.Name == "month")
                        {
                            date = DateTime.Parse(xmlReader.Value);
                        }
                    }
                }
                else if (xmlReader.NodeType == XmlNodeType.Text)
                {
                    //decimal expenses = decimal.Parse(xmlReader.Value.Replace('.', ','));
                    decimal expenses = decimal.Parse(xmlReader.Value);
                    var data = new XmlVendorExpenseEntry(storeName, date, expenses);
                    xmlData.Add(data);
                }
            }

            return xmlData;
        }
    }
}