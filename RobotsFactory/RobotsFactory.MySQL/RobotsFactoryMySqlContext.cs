namespace RobotsFactory.MySQL
{
    using System;
    using System.Linq;
    using Telerik.OpenAccess;
    using Telerik.OpenAccess.Metadata;

    public partial class RobotsFactoryMySqlContext : OpenAccessContext
    {
        private static MetadataContainer metadataContainer = new RobotsFactoryMySqlMetadataSource().GetModel();
        private static BackendConfiguration backendConfiguration = new BackendConfiguration()
        {
            Backend = "mysql"
        };

        public RobotsFactoryMySqlContext()
            : base(ConnectionStrings.Default.MySqlConnectionString, backendConfiguration, metadataContainer)
        {
        }

        public IQueryable<JsonReport> JsonReports
        {
            get
            {
                return this.GetAll<JsonReport>();
            }
        }

        public void UpdateSchema()
        {
            var handler = this.GetSchemaHandler();
            string script = null;

            try
            {
                script = handler.CreateUpdateDDLScript(null);
            }
            catch
            {
                bool throwException = false;

                try
                {
                    handler.CreateDatabase();
                    script = handler.CreateDDLScript();
                }
                catch
                {
                    throwException = true;
                }

                if (throwException)
                {
                    throw;
                }
            }

            if (string.IsNullOrEmpty(script) == false)
            {
                handler.ExecuteDDLScript(script);
            }
        }
    }
}