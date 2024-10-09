// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenSharpTrace.Abstractions.Persistence;
using OpenSharpTrace.Persistence.SQL.Entities;
using System;
using System.Collections.Generic;

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
        public void InsertMany(List<Trace> entities)
        {
            if (entities == null || entities.Count == 0)
            {
                return;
            }

            try
            {
                var strategy = _context.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        foreach (var entity in entities)
                        {
                            _context.Trace.Add(entity);
                        }
                        _context.SaveChanges();
                        transaction.Commit();
                    }                    
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.Message);
            }
        }
    }
}
