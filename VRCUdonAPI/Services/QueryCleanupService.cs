using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRCUdonAPI.Models.Entities;
using VRCUdonAPI.Models.Settings;
using VRCUdonAPI.Repositories;

namespace VRCUdonAPI.Services
{
    public class QueryCleanupService : IHostedService, IDisposable
    {
        public Timer Timer { get; private set; }
        public readonly QuerySettings Settings;
        private readonly IServiceProvider Services;

        public QueryCleanupService(IServiceProvider services, QuerySettings querySettings)
        {
            Settings = querySettings;
            Services = services;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            Timer = new Timer(CheckForForgottenQueries, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        private async void CheckForForgottenQueries(object state)
        {
            using (var scope = Services.CreateScope())
            {
                QueryContext context = scope.ServiceProvider.GetRequiredService<QueryContext>();

                List<Query> outdatedQueries = await context.Queries.Where(q => (DateTime.UtcNow - q.WhenUpdated).Seconds > Settings.RemovalThresholdInSeconds).ToListAsync();

                if (outdatedQueries.Count() != 0)
                {
                    foreach (Query query in outdatedQueries)
                    {
                        context.Queries.Remove(query);
                    }

                    await context.SaveChangesAsync();
                }
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            Timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Timer?.Dispose();
        }
    }
}
