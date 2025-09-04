// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.DependencyInjection;
using OpenSharpTrace.Abstractions.Persistence;
using OpenSharpTrace.Persistence.SQL.Entities;
using OpenSharpTrace.TransactionQueue;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenSharpTrace.TransactionScheduler
{
    public class ServiceTransaction
    {
        private readonly ITraceQueue<Trace> _transactionQueue;
        private readonly IServiceScopeFactory _scopeFactory;

        public ServiceTransaction(
            ITraceQueue<Trace> transactionQueue,
            IServiceScopeFactory scopeFactory)
        {
            _transactionQueue = transactionQueue;
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Persists all tracks collected since the last run
        /// </summary>
        /// <returns></returns>
        public async Task WriteTraceFromQueueAsync()
        {
            var currentTraceList = new List<Trace>();
            while (_transactionQueue.TryDequeue(out var t))
                currentTraceList.Add(t);

            if (currentTraceList.Count == 0) return;

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ISqlTraceRepository>();
            await repo.InsertManyAsync(currentTraceList);
        }
    }
}