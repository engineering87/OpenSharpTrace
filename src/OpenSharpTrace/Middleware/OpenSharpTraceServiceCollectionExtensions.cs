// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenSharpTrace.Abstractions.Persistence;
using OpenSharpTrace.Persistence.SQL;
using OpenSharpTrace.Persistence.SQL.Entities;
using OpenSharpTrace.TransactionQueue;
using OpenSharpTrace.TransactionScheduler;
using System;
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
               .AddJsonFile("appsettings.json", false, true)
               .AddEnvironmentVariables()
               .Build();

            var connectionString = configuration.GetConnectionString("TraceDb");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The connection string 'TraceDb' is not configured.");
            }

            collection.AddDbContext<TraceContext>(options =>
            {
                options.UseSqlServer(connectionString,
                 sqlServerOptionsAction: sqlOptions =>
                 {
                     sqlOptions.EnableRetryOnFailure();
                 });
            });
            collection.AddScoped<ISqlTraceRepository, SqlTraceRepository>();
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
               .AddJsonFile("appsettings.json", false, true)
               .AddEnvironmentVariables()
               .Build();

            var connectionString = configuration.GetConnectionString(connectionKey);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"The connection string '{connectionKey}' is not configured.");
            }

            collection.AddDbContext<TraceContext>(options =>
            {
                options.UseSqlServer(connectionString,
                 sqlServerOptionsAction: sqlOptions =>
                 {
                     sqlOptions.EnableRetryOnFailure();
                 });
            });
            collection.AddScoped<ISqlTraceRepository, SqlTraceRepository>();
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
               .AddJsonFile(jsonFileName, false, true)
               .AddEnvironmentVariables()
               .Build();

            var connectionString = configuration.GetConnectionString(connectionKey);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"The connection string '{connectionKey}' is not configured.");
            }

            collection.AddDbContext<TraceContext>(options =>
            {
                options.UseSqlServer(connectionString,
                 sqlServerOptionsAction: sqlOptions =>
                 {
                     sqlOptions.EnableRetryOnFailure();
                 });
            });
            collection.AddScoped<ISqlTraceRepository, SqlTraceRepository>();
            collection.AddSingleton<ITraceQueue<Trace>, TraceQueue<Trace>>();
            collection.AddSingleton<ServiceTransaction>();
            collection.AddHostedService<ScheduledServiceTransaction>();
        }
    }
}
