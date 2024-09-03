// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.EntityFrameworkCore;
using OpenSharpTrace.Persistence.SQL.Entities;
using System;

namespace OpenSharpTrace.Persistence.SQL
{
    public class TraceContext : DbContext, IDisposable
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        private void EnsureTraceTableExists()
        {
            using (var connection = Database.GetDbConnection())
            {
                try
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Trace')
                            BEGIN
                                CREATE TABLE [dbo].[Trace](
	                                [Id] [bigint] IDENTITY(1,1) NOT NULL,
	                                [TransactionId] [nvarchar](36) NULL,
	                                [ServerId] [nvarchar](MAX) NULL,
	                                [ClientId] [nvarchar](MAX) NULL,
	                                [HttpMethod] [nvarchar](7) NULL,
	                                [HttpPath] [nvarchar](MAX) NULL,
	                                [HttpStatusCode] [int] NULL,
	                                [ActionDescriptor] [nvarchar](MAX) NULL,
	                                [RemoteAddress] [nvarchar](MAX) NULL,
	                                [JsonRequest] [nvarchar](MAX) NULL,
	                                [JsonResponse] [nvarchar](MAX) NULL,
	                                [TimeStamp] [datetime2](7) NULL,
	                                [Exception] [nvarchar](MAX) NULL,
	                                [ExecutionTime] [numeric] NULL,
                                ) ON [PRIMARY];
                            END";
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    // database is not ready or the connectionstring is wrong
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
