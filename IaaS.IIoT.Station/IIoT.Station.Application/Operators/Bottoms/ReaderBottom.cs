namespace Station.Application.Operators.Bottoms;
internal sealed class ReaderBottom : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(Menu.RefreshTime).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                if (StructuralEngine.EnableStorage) await Task.WhenAll(new[]
                {
                    AbstractPool.SetElectricityStatisticAsync(),
                    AbstractPool.SetSpindleSpeedOdometerChartAsync(),
                    AbstractPool.SetSpindleThermalCompensationChartAsync()
                });
                if (Histories.Any()) Histories.Clear();
                FoundationPool.PushReaderBottom(DateTime.UtcNow);
            }
            catch (Exception e)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    HistoryEngine.Record(new IHistoryEngine.FavorerPayload
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
    public required IAbstractPool AbstractPool { get; init; }
    public required IHistoryEngine HistoryEngine { get; init; }
    public required IFoundationPool FoundationPool { get; init; }
    public required IStructuralEngine StructuralEngine { get; init; }
}