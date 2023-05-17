namespace Platform.Application.Supports.Carriers;
internal sealed class EnvironmentLayout : BackgroundService
{
    readonly IBaseLoader _baseLoader;
    readonly IInfluxExpert _influxExpert;
    readonly IFoundationPool _foundationPool;
    public EnvironmentLayout(
        IBaseLoader baseLoader,
        IInfluxExpert influxExpert,
        IFoundationPool foundationPool)
    {
        _baseLoader = baseLoader;
        _influxExpert = influxExpert;
        _foundationPool = foundationPool;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _baseLoader.RefreshProfileAsync();
            await _baseLoader.Transport.StartAsync();
            if (_baseLoader.Profile is not null) await _baseLoader.OverwriteProfileAsync(_baseLoader.Profile);
            while (await new PeriodicTimer(TimeSpan.FromSeconds(1)).WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await _baseLoader.RefreshProfileAsync();
                    if (_baseLoader.Profile is not null)
                    {
                        foreach (IInfluxExpert.BucketTag item in Enum.GetValues(typeof(IInfluxExpert.BucketTag)))
                        {
                            var name = item.GetType().GetDescription(item.ToString());
                            await _influxExpert.InitialDataPoolAsync(name);
                        }
                        _influxExpert.Enabled = true;
                    }
                    if (Histories.Any()) Histories.Clear();
                    _foundationPool.PushShellerBottom(DateTime.UtcNow);
                }
                catch (Exception e)
                {
                    _influxExpert.Enabled = false;
                    if (!Histories.Contains(e.Message))
                    {
                        Histories.Add(e.Message);
                        _baseLoader.Record(RecordType.BasicSettings, new()
                        {
                            Title = $"{nameof(EnvironmentLayout)}.{nameof(ExecuteAsync)}",
                            Name = "InfluxDB",
                            Message = e.Message
                        });
                    }
                }
            }
        }
        catch (Exception e)
        {
            if (!Histories.Contains(e.Message))
            {
                Histories.Add(e.Message);
                Log.Fatal(Menu.Title, nameof(EnvironmentLayout), new { e.Message, e.StackTrace });
            }
        }
    }
    List<string> Histories { get; init; } = new();
}