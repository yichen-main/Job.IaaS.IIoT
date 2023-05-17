namespace Platform.Application.Supports.Carriers;
internal sealed class DataCalculator : BackgroundService
{
    readonly IBaseLoader _baseLoader;
    readonly ITimelineWrapper _timelineWrapper;
    public DataCalculator(IBaseLoader baseLoader, ITimelineWrapper timelineWrapper)
    {
        _baseLoader = baseLoader;
        _timelineWrapper = timelineWrapper;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(TimeSpan.FromMinutes(1)).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                var oneDayMachineStatusMinutes = _timelineWrapper.RootInformation.OneDayMachineStatusMinutes();


                if (Histories.Any()) Histories.Clear();
            }
            catch (Exception e)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    _baseLoader.Record(RecordType.BasicSettings, new()
                    {
                        Title = $"{nameof(DataCalculator)}.{nameof(ExecuteAsync)}",
                        Name = "Statistics",
                        Message = e.Message
                    });
                }
            }
        }
    }
    List<string> Histories { get; init; } = new();
}