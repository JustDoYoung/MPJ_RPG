
using AccountDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AccountServer.Services
{
    public class TimedHostedService : IHostedService
    {
        Timer? _timer = null;
        //AccountDbContext _dbContext;
        private readonly IServiceScopeFactory _scopeFactory;

        public TimedHostedService(IServiceScopeFactory scopeFactory)
        {
            //_dbContext = dbContext;
            _scopeFactory = scopeFactory;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextFactory<AccountDbContext>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //10초마다 DoWork 실행
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        void DoWork(object? state)
        {
            // Discard the result
            _ = DoWorkAsync();
        }

        private async Task DoWorkAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AccountDbContext>();

                var rankers = await dbContext.Rankings.
                   OrderByDescending(t => t.Score)
                   .Skip(3)
                   .ToListAsync();

                dbContext.Rankings.RemoveRange(rankers);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
