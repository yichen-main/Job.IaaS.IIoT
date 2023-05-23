namespace Platform.Application.Supports.Carriers;
internal sealed class EnvironmentLayout(IBaseLoader baseLoader, IInitialService initialService, IMessagePublisher messagePublisher) : BackgroundService
{
    readonly IBaseLoader _baseLoader = baseLoader;
    readonly IInitialService _initialService = initialService;
    readonly IMessagePublisher _messagePublisher = messagePublisher;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        PeriodicTimer initializer = new(TimeSpan.FromSeconds(1));
        while (await initializer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                if (!_baseLoader.Transport.IsStarted)
                {
                    await _baseLoader.Transport.StartAsync();
                    await _initialService.CreateSwitchFileAsync();
                }
                await _baseLoader.RefreshProfileAsync();
                if (_baseLoader.Profile is not null)
                {
                    await _baseLoader.OverwriteProfileAsync(_baseLoader.Profile);
                    foreach (IInfluxExpert.BucketTag item in Enum.GetValues(typeof(IInfluxExpert.BucketTag)))
                    {
                        var name = item.GetType().GetDescription(item.ToString());
                        await _baseLoader.InitialStorageAsync(name);
                    }
                    _baseLoader.StorageEnabled = true;
                    await _messagePublisher.RawCalculation.StatisticsUnitDayAsync();
                    initializer.Dispose();
                }
            }
            catch (InfluxException e)
            {
                _baseLoader.StorageEnabled = false;
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    _baseLoader.Record(RecordType.BasicSettings, new()
                    {
                        Title = $"{nameof(EnvironmentLayout)}.{nameof(ExecuteAsync)}",
                        Name = nameof(InfluxException),
                        Message = e.Message
                    });
                }
            }
            catch (Exception e)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    _baseLoader.Record(RecordType.BasicSettings, new()
                    {
                        Title = $"{nameof(EnvironmentLayout)}.{nameof(ExecuteAsync)}",
                        Name = nameof(PeriodicTimer),
                        Message = e.Message
                    });
                }
            }
        }
        while (await new PeriodicTimer(TimeSpan.FromSeconds(15)).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await _baseLoader.RefreshProfileAsync();
                if (Histories.Any()) Histories.Clear();
            }
            catch (Exception e)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    _baseLoader.Record(RecordType.BasicSettings, new()
                    {
                        Title = $"{nameof(EnvironmentLayout)}.{nameof(ExecuteAsync)}",
                        Name = nameof(Exception),
                        Message = e.Message
                    });
                }
            }
        }
    }
    List<string> Histories { get; init; } = new();
}