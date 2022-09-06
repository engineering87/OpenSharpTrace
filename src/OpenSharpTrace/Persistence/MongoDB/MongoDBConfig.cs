// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using OpenSharpTrace.Abstractions.Configuration;
using OpenSharpTrace.Enums;

namespace OpenSharpTrace.Persistence.MongoDB
{
    public class MongoDBConfig : ISharpTraceConfig
    {
        public string ConnectionString { get; }

        public string CollectionName { get; }
        public string DatabaseName { get; }

        public MongoDBConfig(IConfiguration section)
        {
            var sectionChildren = section.GetChildren();
            var configurationSections = sectionChildren.ToList();

            var sectionChildrenConnectionString = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.Connectionstring.ToString());
            ConnectionString = sectionChildrenConnectionString?.Value;

            var sectionChildrenDatabaseName = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.DatabaseName.ToString());
            DatabaseName = sectionChildrenDatabaseName?.Value;

            var sectionChildrenCollectionName = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.CollectionName.ToString());
            CollectionName = sectionChildrenCollectionName?.Value;
        }
    }
}
