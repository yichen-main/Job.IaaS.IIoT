namespace Station.Application.Operators.Bottoms;
internal sealed class ReaderBottom : BackgroundService
{
    readonly IAbstractPool _abstractPool;
    readonly IHistoryEngine _historyEngine;
    readonly IFoundationPool _foundationPool;
    readonly IStructuralEngine _structuralEngine;
    public ReaderBottom(
        IAbstractPool abstractPool,
        IHistoryEngine historyEngine,
        IFoundationPool foundationPool,
        IStructuralEngine structuralEngine)
    {
        _abstractPool = abstractPool;
        _historyEngine = historyEngine;
        _foundationPool = foundationPool;
        _structuralEngine = structuralEngine;

    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(Menu.RefreshTime).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                if (_structuralEngine.EnableStorage) await Task.WhenAll(new[]
                {
                    _abstractPool.SetElectricityStatisticAsync(),
                    _abstractPool.SetSpindleSpeedOdometerChartAsync(),
                    _abstractPool.SetSpindleThermalCompensationChartAsync()
                });
                if (Histories.Any()) Histories.Clear();
                _foundationPool.PushReaderBottom(DateTime.UtcNow);
            }
            catch (Exception e)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    _historyEngine.Record(new IHistoryEngine.FavorerPayload
                    {
                        Name = nameof(ReaderBottom),
                        Message = e.Message,
                        Source = e.Source
                    });
                }
            }
        }
    }
    internal required List<string> Histories { get; init; } = new();
}