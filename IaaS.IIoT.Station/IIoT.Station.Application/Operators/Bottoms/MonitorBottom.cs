namespace Station.Application.Operators.Bottoms;
internal sealed class MonitorBottom : BackgroundService
{
    readonly IHistoryEngine _historyEngine;
    readonly IFoundationPool _foundationPool;
    public MonitorBottom(IHistoryEngine historyEngine, IFoundationPool foundationPool)
    {
        _historyEngine = historyEngine;
        _foundationPool = foundationPool;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(Menu.RefreshTime).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                if (Histories.Any()) Histories.Clear();
                _foundationPool.PushMonitorBottom(DateTime.UtcNow);
            }
            catch (Exception e)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    _historyEngine.Record(new IHistoryEngine.FavorerPayload
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
}