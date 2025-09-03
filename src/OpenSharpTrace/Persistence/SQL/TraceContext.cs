// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.EntityFrameworkCore;
using OpenSharpTrace.Persistence.SQL.Entities;
using System;

namespace OpenSharpTrace.Persistence.SQL
{
    public class TraceContext : DbContext, IDisposable
    {
        public TraceContext(DbContextOptions options)
            : base(options)
        {
            try
            {
                Database.EnsureCreated();

                EnsureTraceTableExists();
            }
            catch
            {
                // database is not ready or the connectionstring is wrong
            }
        }

        public virtual DbSet<Trace> Trace { get; set; }

        private void EnsureTraceTableExists()
        {
            try
            {
                Database.ExecuteSqlRaw(@"
            -- Create the Trace table if it does not exist
            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Trace')
            BEGIN
                CREATE TABLE [dbo].[Trace](
	                [Id] [bigint] IDENTITY(1,1) NOT NULL,
	                [TransactionId] [nvarchar](MAX) NULL,
	                [ServerId] [nvarchar](MAX) NULL,
	                [ClientId] [nvarchar](MAX) NULL,
	                [HttpMethod] [nvarchar](16) NULL,
	                [HttpPath] [nvarchar](MAX) NULL,
	                [HttpStatusCode] [int] NULL,
	                [ActionDescriptor] [nvarchar](MAX) NULL,
	                [RemoteAddress] [nvarchar](MAX) NULL,
	                [JsonRequest] [nvarchar](MAX) NULL,
	                [JsonResponse] [nvarchar](MAX) NULL,
	                [TimeStamp] [datetime2](7) NULL,
	                [Exception] [nvarchar](MAX) NULL,
	                [ExecutionTime] [decimal](18,3) NULL,
                    CONSTRAINT [PK_Trace] PRIMARY KEY CLUSTERED ([Id] ASC)
                ) ON [PRIMARY];
            END;

            -- Index for chronological queries (sorting/filtering by timestamp)
            IF NOT EXISTS (
                SELECT 1 FROM sys.indexes 
                WHERE name = 'IX_Trace_TimeStamp' 
                  AND object_id = OBJECT_ID(N'[dbo].[Trace]')
            )
            BEGIN
                CREATE NONCLUSTERED INDEX [IX_Trace_TimeStamp]
                ON [dbo].[Trace]([TimeStamp]);
            END;

            -- Index for client-based queries
            IF NOT EXISTS (
                SELECT 1 FROM sys.indexes 
                WHERE name = 'IX_Trace_ClientId' 
                  AND object_id = OBJECT_ID(N'[dbo].[Trace]')
            )
            BEGIN
                CREATE NONCLUSTERED INDEX [IX_Trace_ClientId]
                ON [dbo].[Trace]([ClientId]);
            END;

            -- Index for transaction-based queries
            IF NOT EXISTS (
                SELECT 1 FROM sys.indexes 
                WHERE name = 'IX_Trace_TransactionId' 
                  AND object_id = OBJECT_ID(N'[dbo].[Trace]')
            )
            BEGIN
                CREATE NONCLUSTERED INDEX [IX_Trace_TransactionId]
                ON [dbo].[Trace]([TransactionId]);
            END;

            -- Index for HTTP status code analysis
            IF NOT EXISTS (
                SELECT 1 FROM sys.indexes 
                WHERE name = 'IX_Trace_HttpStatusCode' 
                  AND object_id = OBJECT_ID(N'[dbo].[Trace]')
            )
            BEGIN
                CREATE NONCLUSTERED INDEX [IX_Trace_HttpStatusCode]
                ON [dbo].[Trace]([HttpStatusCode]);
            END;
        ");
            }
            catch
            {
                // database is not ready or the connection string is wrong
            }
        }
    }
}
