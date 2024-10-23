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
            if(_transactionQueue.Count() == 0) return;

            using (var scope = _scopeFactory.CreateScope())
            {
                var currentTraceList = new List<Trace>();

                while (_transactionQueue.Count() > 0)
                {
                    currentTraceList.Add(_transactionQueue.Dequeue());
                }

                var sqlTraceRepository = scope.ServiceProvider.GetRequiredService<ISqlTraceRepository>();
                await sqlTraceRepository.InsertManyAsync(currentTraceList);
            }

            return;
        }
    }
}
