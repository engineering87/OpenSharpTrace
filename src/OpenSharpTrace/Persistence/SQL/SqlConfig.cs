// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using OpenSharpTrace.Abstractions.Configuration;
using OpenSharpTrace.Enums;

namespace OpenSharpTrace.Persistence.SQL
{
    public class SqlConfig : ISharpTraceConfig
    {
        public string ConnectionString { get; }

        public string CollectionName { get; private set; }
        public string DatabaseName { get; private set; }

        public SqlConfig(IConfiguration section)
        {
            var sectionChildren = section.GetChildren();
            var configurationSections = sectionChildren.ToList();

            var sectionChildrenConnectionString = configurationSections.FirstOrDefault(s => s.Key.ToLower() == AppConfigParameterEnums.Connectionstring.ToString());
            ConnectionString = sectionChildrenConnectionString?.Value;
        }
    }
}
