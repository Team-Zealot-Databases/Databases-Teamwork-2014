namespace RobotsFactory.Common
{
    using System;
    using System.Linq;
    using Ionic.Zip;

    public class ZipFileProcessor
    {
        public void Extract(string zipToUnpack, string unpackDirectory)
        {
            using (var zipFile = ZipFile.Read(zipToUnpack))
            {
                foreach (var entity in zipFile)
                {
                    entity.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }
    }
}