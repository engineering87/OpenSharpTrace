using Microsoft.Extensions.Configuration;
using OpenSharpTrace.Abstractions.Configuration;
using OpenSharpTrace.Abstractions.Persistence;
using OpenSharpTrace.Enums;
using OpenSharpTrace.Persistence.MongoDB;
using OpenSharpTrace.Persistence.SQL;

namespace OpenSharpTrace.Persistence
{
    public class ConnectorFactory<T>
    {
        private readonly IConfigurationSection _section;

        public ConnectorFactory(IConfigurationSection section)
        {
            this._section = section;
        }

        /// <summary>
        /// Get the configuration strategy for the connector type.
        /// </summary>
        /// <param name="section">The configuration section.</param>
        /// <param name="connectorTypes">The connector type.</param>
        /// <returns></returns>
        public virtual ISharpTraceConfig GetConfigurationStrategy(IConfigurationSection section, ConnectorTypeEnums connectorTypes)
        {
            return connectorTypes switch
            {
                ConnectorTypeEnums.SQL => new SqlConfig(section),
                ConnectorTypeEnums.MongoDB => new MongoDBConfig(section),
                _ => section.Get<ISharpTraceConfig>()
            };
        }

        //public IRepository<T> GetStrategy()
        //{
        //    var dbType = _section.GetChildren().FirstOrDefault(s => s.Key.ToLower() == "instance")?.Value;

        //    if (!Enum.TryParse(dbType, true, out ConnectorTypeEnums connectorTypes))
        //        throw new Exception("Instance section for OpenSharpTrace was not found.");

        //    var connectorConfig = GetConfigurationStrategy(_section, connectorTypes);
        //    switch (connectorTypes)
        //    {
        //        case ConnectorTypeEnums.SQL:
        //            {
        //                var sqlConfig = connectorConfig as SqlConfig;
        //                return new SqlTraceRepository(sqlConfig);
        //            }
        //        case ConnectorTypeEnums.MongoDB:
        //            {
        //                var mongoDbConfig = connectorConfig as MongoDBConfig;
        //                return new MongoDBTraceRepository(mongoDbConfig);
        //            }
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //}
    }
}
