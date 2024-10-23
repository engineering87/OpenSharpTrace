// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenSharpTrace.Abstractions.Persistence;
using OpenSharpTrace.Persistence.SQL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenSharpTrace.Persistence.SQL
{
    public class SqlTraceRepository : ISqlTraceRepository
    {
        private readonly TraceContext _context;

        private readonly ILogger _logger;

        public SqlTraceRepository(ILoggerFactory loggerFactory, TraceContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = loggerFactory.CreateLogger(GetType().ToString());
        }
        
        /// <summary>
        /// Write the current trace entities
        /// </summary>
        /// <param name="entities"></param>
        public async Task InsertManyAsync(List<Trace> entities)
        {
            if (entities == null || entities.Count == 0)
            {
                return;
            }

            try
            {
                var strategy = _context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            await _context.Trace.AddRangeAsync(entities);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, "An error occurred while committing the transaction.");
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while inserting trace entities.");
            }
        }
    }
}
