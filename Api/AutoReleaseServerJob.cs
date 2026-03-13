using Application.Interfaces;

namespace Api
{
    public class AutoReleaseServerJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AutoReleaseServerJob> _logger;

        public AutoReleaseServerJob(IServiceProvider serviceProvider, ILogger<AutoReleaseServerJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IServerRepository>();

                    var threshold = DateTime.UtcNow.AddMinutes(-20);
                    var expiredServers = await repository.GetExpiredRentedServersAsync(threshold);

                    foreach (var server in expiredServers)
                    {
                        server.Release();
                        await repository.UpdateAsync(server);
                        _logger.LogInformation("Сработал таймер 20 минут! Сервер {Id} автоматически изъят у пользователя и освобожден.", server.Id);
                    }

                    if (expiredServers.Any())
                    {
                        await repository.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при выполнении фоновой задачи авто-освобождения серверов.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}