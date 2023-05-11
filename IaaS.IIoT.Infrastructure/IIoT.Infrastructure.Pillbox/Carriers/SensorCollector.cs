namespace Infrastructure.Pillbox.Carriers;
internal sealed class SensorCollector : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(TimeSpan.FromSeconds(1)).WaitForNextTickAsync(stoppingToken))
        {
            try
            {

            }
            catch (Exception e)
            {
                Log.Error("[{0}] {1}", nameof(SensorCollector), new { e.Message, e.StackTrace });
            }
        }
    }
}