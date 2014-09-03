namespace RobotsFactory.MySQL
{
    using System.Collections.Generic;
    using Telerik.OpenAccess.Metadata;
    using Telerik.OpenAccess.Metadata.Fluent;

    public class RobotsFactoryMySqlMetadataSource : FluentMetadataSource
    {
        private const string JsonReportsTableName = "JsonReports";

        protected override IList<MappingConfiguration> PrepareMapping()
        {
            var configurations = new List<MappingConfiguration>();
            var productConfiguration = new MappingConfiguration<JsonReport>();

            productConfiguration.MapType(x => new
            {
                ReportId = x.ReportId,
                JsonFileName = x.JsonFileName,
                JsonContent = x.JsonContent
            }).ToTable(JsonReportsTableName);

            productConfiguration.HasProperty(x => x.ReportId).IsIdentity(KeyGenerator.Autoinc);
            configurations.Add(productConfiguration);

            return configurations;
        }
    }
}