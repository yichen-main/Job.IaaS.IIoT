namespace Platform.Application.Supports.Carriers;
internal sealed class ComplexOperator(IBaseLoader baseLoader, IRawCalculation rawCalculation) : BackgroundService
{
    readonly IBaseLoader _baseLoader = baseLoader;
    readonly IRawCalculation _rawCalculation = rawCalculation;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (await new PeriodicTimer(TimeSpan.FromMinutes(1)).WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    if (_baseLoader.StorageEnabled)
                    {
                        _rawCalculation.SstatisticsUnitDay();
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
        catch (Exception e)
        {
            Log.Fatal(Menu.Title, nameof(ComplexOperator), new { e.Message, e.StackTrace });
        }
    }
    List<string> Histories { get; init; } = new();
}