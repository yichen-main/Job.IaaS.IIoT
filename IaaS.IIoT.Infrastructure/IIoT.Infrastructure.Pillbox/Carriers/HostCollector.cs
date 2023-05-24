namespace Infrastructure.Pillbox.Carriers;
internal sealed class HostCollector(IDataTransfer dataTransfer) : BackgroundService
{
    readonly IDataTransfer _dataTransfer = dataTransfer;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(TimeSpan.FromSeconds(1)).WaitForNextTickAsync(stoppingToken))
        {
            await Task.WhenAll(new Task[]
            {
                _dataTransfer.PushControllerAsync(),
                _dataTransfer.PushMerchandiseAsync(stoppingToken)
            });
        }
    }
}