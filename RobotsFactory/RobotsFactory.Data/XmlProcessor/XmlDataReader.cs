namespace RobotsFactory.Data.XMLProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    public class XmlDataReader
    {
        public IList<XmlData> ReadXmlReportsData(string dataSourcePath)
        {
            XmlTextReader xmlReader = new XmlTextReader(dataSourcePath);
            List<XmlData> xmlData = new List<XmlData>();
            string storeName = string.Empty;
            DateTime date = new DateTime();

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
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

                        break;
                    case XmlNodeType.Text:
                        decimal expenses = decimal.Parse(xmlReader.Value.Replace('.', ','));
                        var data = new XmlData(storeName, date, expenses);
                        xmlData.Add(data);
                        break;
                }
            }

            return xmlData;
        }
    }
}
