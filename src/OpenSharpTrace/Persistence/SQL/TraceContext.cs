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
            }
            catch
            {
                // database is not ready or the connectionstring is wrong
                // ignore
            }
        }

        public virtual DbSet<Trace> Trace { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
