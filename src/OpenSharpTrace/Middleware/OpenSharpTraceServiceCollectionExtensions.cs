// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenSharpTrace.Abstractions.Persistence;
using OpenSharpTrace.Persistence.SQL;
using OpenSharpTrace.Persistence.SQL.Entities;
using OpenSharpTrace.TraceQueue;
using OpenSharpTrace.TransactionScheduler;
using System.IO;

namespace OpenSharpTrace.Middleware
{
    public static class OpenSharpTraceServiceCollectionExtensions
    {
        /// <summary>
        /// Register the OpenSharpTrace middleware to enable traces
        /// </summary>
        /// <param name="collection"></param>
        public static void RegisterOpenSharpTrace(this IServiceCollection collection)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", true, true)
               .AddEnvironmentVariables()
               .Build();

            collection.AddScoped<ISqlTraceRepository, SqlTraceRepository>();
            collection.AddDbContext<TraceContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("TraceDb"),
                 sqlServerOptionsAction: sqlOptions =>
                 {
                     sqlOptions.EnableRetryOnFailure();
                 });
            });
            collection.AddSingleton<ITraceQueue<Trace>, TraceQueue<Trace>>();
            collection.AddSingleton<ServiceTransaction>();
            collection.AddHostedService<ScheduledServiceTransaction>();
        }

        /// <summary>
        /// Register the OpenSharpTrace middleware to enable traces
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="connectionKey">Key of the connection string for trace persistence</param>
        public static void RegisterOpenSharpTrace(this IServiceCollection collection, string connectionKey)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", true, true)
               .AddEnvironmentVariables()
               .Build();

            collection.AddScoped<ISqlTraceRepository, SqlTraceRepository>();
            collection.AddDbContext<TraceContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(connectionKey),
                 sqlServerOptionsAction: sqlOptions =>
                 {
                     sqlOptions.EnableRetryOnFailure();
                 });
            });
            collection.AddSingleton<ITraceQueue<Trace>, TraceQueue<Trace>>();
            collection.AddSingleton<ServiceTransaction>();
            collection.AddHostedService<ScheduledServiceTransaction>();
        }

        /// <summary>
        /// Register the OpenSharpTrace middleware to enable traces
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="connectionKey">Key of the connection string for trace persistence</param>
        /// <param name="jsonFileName">JSON file configuration name</param>
        public static void RegisterOpenSharpTrace(this IServiceCollection collection, string connectionKey, string jsonFileName)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile(jsonFileName, true, true)
               .AddEnvironmentVariables()
               .Build();

            collection.AddScoped<ISqlTraceRepository, SqlTraceRepository>();
            collection.AddDbContext<TraceContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(connectionKey),
                 sqlServerOptionsAction: sqlOptions =>
                 {
                     sqlOptions.EnableRetryOnFailure();
                 });
            });
            collection.AddSingleton<ITraceQueue<Trace>, TraceQueue<Trace>>();
            collection.AddSingleton<ServiceTransaction>();
            collection.AddHostedService<ScheduledServiceTransaction>();
        }
    }
}
