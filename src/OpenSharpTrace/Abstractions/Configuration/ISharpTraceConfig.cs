// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace OpenSharpTrace.Abstractions.Configuration
{
    public interface ISharpTraceConfig
    {
        string ConnectionString { get; }
        string CollectionName { get; }
        string DatabaseName { get; }
    }
}
