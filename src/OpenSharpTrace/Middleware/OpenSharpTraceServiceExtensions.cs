// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenSharpTrace.Abstractions.Persistence;
using OpenSharpTrace.Persistence.SQL;
using Microsoft.EntityFrameworkCore;

namespace OpenSharpTrace.Middleware
{
    public static class OpenSharpTraceServiceExtensions
    {
        public static void RegisterOpenSharpTrace(this IServiceCollection collection)
        {
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", true, true)
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
        }
    }
}
