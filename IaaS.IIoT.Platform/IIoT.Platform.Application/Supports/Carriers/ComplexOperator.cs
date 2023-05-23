namespace Platform.Application.Supports.Carriers;
internal sealed class ComplexOperator(IBaseLoader baseLoader, IMessagePublisher messagePublisher) : BackgroundService
{
    readonly IBaseLoader _baseLoader = baseLoader;
    readonly IMessagePublisher _messagePublisher = messagePublisher;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(TimeSpan.FromMinutes(10)).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                if (_baseLoader.StorageEnabled)
                {
                    await _messagePublisher.RawCalculation.StatisticsUnitDayAsync();
                }
                if (Histories.Any()) Histories.Clear();
            }
            catch (Exception e)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    _baseLoader.Record(RecordType.BasicSettings, new()
                    {
                        Title = $"{nameof(ComplexOperator)}.{nameof(ExecuteAsync)}",
                        Name = "Statistics",
                        Message = e.Message
                    });
                }
            }
        }
    }
    List<string> Histories { get; init; } = new();
}