// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenSharpTrace.Abstractions.Persistence;
using OpenSharpTrace.Persistence.SQL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task InsertManyAsync(IEnumerable<Trace> entities)
        {
            var list = entities as ICollection<Trace> ?? entities?.ToList();
            if (list is null || list.Count == 0) return;

            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await _context.Trace.AddRangeAsync(list);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to insert {Count} trace entities.", list.Count);
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }
    }
}
