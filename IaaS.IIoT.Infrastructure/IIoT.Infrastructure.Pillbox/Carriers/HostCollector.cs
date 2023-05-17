namespace Infrastructure.Pillbox.Carriers;
internal sealed class HostCollector : BackgroundService
{
    readonly IDataTransfer _dataTransfer;
    public HostCollector(IDataTransfer dataTransfer) => _dataTransfer = dataTransfer;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(TimeSpan.FromSeconds(1)).WaitForNextTickAsync(stoppingToken))
        {
            await Task.WhenAll(new Task[]
            {
                _dataTransfer.ControllerAsync(),
                _dataTransfer.MerchandiseAsync()
            });
        }
    }
}