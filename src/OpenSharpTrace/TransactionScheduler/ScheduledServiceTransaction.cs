// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenSharpTrace.TransactionScheduler
{
    public class ScheduledServiceTransaction : IHostedService
    {
        private readonly IServiceProvider _services;

        private Timer _timer;

        public ScheduledServiceTransaction(
            IServiceProvider services)
        {
            _services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using var scope = _services.CreateScope();
            var serviceTransaction = scope.ServiceProvider.GetRequiredService<ServiceTransaction>();
            await serviceTransaction.WriteTraceFromQueue();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
