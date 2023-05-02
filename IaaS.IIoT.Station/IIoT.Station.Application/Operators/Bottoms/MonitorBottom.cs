namespace Station.Application.Operators.Bottoms;
internal sealed class MonitorBottom : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(Menu.RefreshTime).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                if (Histories.Any()) Histories.Clear();
                FoundationPool.PushMonitorBottom(DateTime.UtcNow);
            }
            catch (Exception e)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    HistoryEngine.Record(new IHistoryEngine.FavorerPayload
                    {
                        Name = nameof(MonitorBottom),
                        Message = e.Message,
                        Source = e.Source
                    });
                }
            }
        }
    }
    internal required List<string> Histories { get; init; } = new();
    public required IHistoryEngine HistoryEngine { get; init; }
    public required IFoundationPool FoundationPool { get; init; }
}