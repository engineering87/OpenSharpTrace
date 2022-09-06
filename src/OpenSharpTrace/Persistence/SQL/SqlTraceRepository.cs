// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Logging;
using OpenSharpTrace.Abstractions.Persistence;
using OpenSharpTrace.Persistence.SQL.Entities;

namespace OpenSharpTrace.Persistence.SQL
{
    public class SqlTraceRepository : ISqlTraceRepository//, IRepository<Trace>
    {
        private readonly TraceContext _context;

        private readonly ILogger _logger;

        public SqlTraceRepository(ILoggerFactory loggerFactory, TraceContext context)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger(GetType().ToString());
        }

        //public SqlTraceRepository(SqlConfig sqlConfig)
        //{
        //    // TODO
        //}

        public void Insert(Trace entity)
        {
            try
            {
                _context.Trace.Add(entity);
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                _logger?.LogError(ex.Message);
            }
        }
    }
}
