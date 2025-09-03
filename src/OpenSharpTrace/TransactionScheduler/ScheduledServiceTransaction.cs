// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenSharpTrace.TransactionScheduler
{
    public class ScheduledServiceTransaction : IHostedService, IDisposable
    {
        private readonly IServiceProvider _services;
        private readonly TimeSpan _interval;
        private readonly SemaphoreSlim _mutex = new(1, 1);
        private Timer _timer;

        public ScheduledServiceTransaction(IServiceProvider services, IConfiguration cfg)
        {
            _services = services;
            var seconds = cfg.GetValue<int?>("ScheduledSharpTrace:TimerIntervalSeconds") ?? 60;
            _interval = TimeSpan.FromSeconds(seconds);
        }

        public Task StartAsync(CancellationToken ct)
        {
            _timer = new Timer(async _ => await Tick(), null, TimeSpan.Zero, _interval);
            return Task.CompletedTask;
        }

        private async Task Tick()
        {
            if (!await _mutex.WaitAsync(0)) return;
            try
            {
                using var scope = _services.CreateScope();
                var tx = scope.ServiceProvider.GetRequiredService<ServiceTransaction>();
                await tx.WriteTraceFromQueueAsync();
            }
            catch (Exception)
            {
                // ignore
            }
            finally
            {
                _mutex.Release();
            }
        }

        public Task StopAsync(CancellationToken ct)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}
