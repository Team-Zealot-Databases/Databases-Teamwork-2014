namespace RobotsFactory.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Win32;

    public static class Utility
    {
        private const string DirectorySearchRegexPattern = @"\d{1,2}-\w{2,3}-\d{4}";

        public static IEnumerable<DirectoryInfo> GetDirectoriesByPattern(string path, string regex = DirectorySearchRegexPattern)
        {
            var dirInfo = new DirectoryInfo(path);
            var matchedDirectories = dirInfo.GetDirectories().Where(d => Regex.IsMatch(d.Name, regex));
            return matchedDirectories;
        }

        public static void CreateDirectoryIfNotExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public static void OpenDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                Process.Start(directoryPath);
            }
        }

        public static DateTime GetSelectedDateOrDefault(string text, DateTime defaultValue)
        {
            if (string.IsNullOrEmpty(text))
            {
                return defaultValue;
            }

            return DateTime.Parse(text);
        }

        public static void SetFileDialogFilter(FileDialog dialog, string type, string defaultFileName = null)
        {
            switch (type)
            {
                case "zip":
                    {
                        dialog.DefaultExt = ".zip";
                        dialog.Filter = "Zip File (.zip)|*.zip";
                        break;
                    }

                case "xlsx":
                    {
                        dialog.DefaultExt = ".xlsx";
                        dialog.Filter = "Excel File (.xlsx)|*.xlsx";
                        break;
                    }

                case "pdf":
                    {
                        dialog.DefaultExt = ".zip";
                        dialog.Filter = "Pdf File (.pdf)|*.pdf";
                        break;
                    }

                case "xml":
                    {
                        dialog.DefaultExt = ".zip";
                        dialog.Filter = "XML File (.xml)|*.xml";
                        break;
                    }

                default:
                    {
                        dialog.FileName = "All files (*.*)|*.*";
                        break;
                    }
            }

            if (!string.IsNullOrEmpty(defaultFileName))
            {
                dialog.FileName = defaultFileName;
            }
        }
    }
}