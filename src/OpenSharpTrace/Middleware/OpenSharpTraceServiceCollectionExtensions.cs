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
        /// Registers the OpenSharpTrace infrastructure using the default
        /// configuration file (<c>appsettings.json</c>) and the connection string
        /// named <c>TraceDb</c>.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="IServiceCollection"/> used for dependency injection.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the <c>TraceDb</c> connection string is missing or empty.
        /// </exception>
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
                     sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: [-2, 1205, 4060, 10928, 10929, 40197, 40501, 40613]
                    );
                 });
            });
            collection.AddScoped<ISqlTraceRepository, SqlTraceRepository>();
            collection.AddSingleton<ITraceQueue<Trace>, TraceQueue<Trace>>();
            collection.AddSingleton<ServiceTransaction>();
            collection.AddHostedService<ScheduledServiceTransaction>();
        }

        /// <summary>
        /// Registers the OpenSharpTrace infrastructure using the specified
        /// connection string key from <c>appsettings.json</c>.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="IServiceCollection"/> used for dependency injection.
        /// </param>
        /// <param name="connectionKey">
        /// The key of the connection string used for trace persistence.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the provided connection string key is not configured.
        /// </exception>
        public static void RegisterOpenSharpTrace(
            this IServiceCollection collection, 
            string connectionKey)
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
                     sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: [-2, 1205, 4060, 10928, 10929, 40197, 40501, 40613]
                    );
                 });
            });
            collection.AddScoped<ISqlTraceRepository, SqlTraceRepository>();
            collection.AddSingleton<ITraceQueue<Trace>, TraceQueue<Trace>>();
            collection.AddSingleton<ServiceTransaction>();
            collection.AddHostedService<ScheduledServiceTransaction>();
        }

        /// <summary>
        /// Registers the OpenSharpTrace infrastructure using the specified
        /// configuration file and connection string key.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="IServiceCollection"/> used for dependency injection.
        /// </param>
        /// <param name="connectionKey">
        /// The key of the connection string used for trace persistence.
        /// </param>
        /// <param name="jsonFileName">
        /// The JSON configuration file containing the connection string.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the provided connection string key is not configured
        /// in the specified JSON file.
        /// </exception>
        public static void RegisterOpenSharpTrace(
            this IServiceCollection collection, 
            string connectionKey, 
            string jsonFileName)
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
                     sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: [-2, 1205, 4060, 10928, 10929, 40197, 40501, 40613]
                    );
                 });
            });
            collection.AddScoped<ISqlTraceRepository, SqlTraceRepository>();
            collection.AddSingleton<ITraceQueue<Trace>, TraceQueue<Trace>>();
            collection.AddSingleton<ServiceTransaction>();
            collection.AddHostedService<ScheduledServiceTransaction>();
        }

        /// <summary>
        /// Registers the OpenSharpTrace infrastructure using the provided
        /// <see cref="IConfiguration"/> and connection string key (default: <c>TraceDb</c>).
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> used for dependency injection.
        /// </param>
        /// <param name="configuration">
        /// The application configuration from which to read the connection string.
        /// </param>
        /// <param name="connectionKey">
        /// The key of the connection string used for trace persistence. Defaults to <c>TraceDb</c>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified connection string key is not configured.
        /// </exception>
        public static void RegisterOpenSharpTrace(
            this IServiceCollection services,
            IConfiguration configuration,
            string connectionKey = "TraceDb")
        {
            var connectionString = configuration.GetConnectionString(connectionKey);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"The connection string '{connectionKey}' is not configured.");
            }

            services.AddDbContext<TraceContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: [-2, 1205, 4060, 10928, 10929, 40197, 40501, 40613]
                    );
                });
            });

            services.AddScoped<ISqlTraceRepository, SqlTraceRepository>();
            services.AddSingleton<ITraceQueue<Trace>, TraceQueue<Trace>>();
            services.AddSingleton<ServiceTransaction>();
            services.AddHostedService<ScheduledServiceTransaction>();
        }
    }
}
