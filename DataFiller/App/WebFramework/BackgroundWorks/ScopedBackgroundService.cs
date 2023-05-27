using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebFramework.BackgroundWorks
{
    public abstract class ScopedBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ScopedBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                await ExecuteInScope(scope.ServiceProvider, stoppingToken);
            }
        }

        public abstract Task ExecuteInScope(IServiceProvider serviceProvider, CancellationToken stoppingToken);
    }


}
