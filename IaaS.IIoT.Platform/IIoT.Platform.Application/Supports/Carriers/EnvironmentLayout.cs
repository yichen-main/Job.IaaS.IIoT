namespace Platform.Application.Supports.Carriers;
internal sealed class EnvironmentLayout : BackgroundService
{
    readonly IBaseLoader _baseLoader;
    readonly IFoundationPool _foundationPool;
    readonly IMessagePublisher _messagePublisher;
    public EnvironmentLayout(
        IBaseLoader baseLoader,
        IFoundationPool foundationPool,
        IMessagePublisher messagePublisher)
    {
        _baseLoader = baseLoader;
        _foundationPool = foundationPool;
        _messagePublisher = messagePublisher;
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
                            await _baseLoader.InitialStorageAsync(name);
                        }
                        _baseLoader.StorageEnabled = true;
                        _messagePublisher.RawCalculation.PushSstatisticsUnitDay();
                        {
                            List<Task> tasks = new()
                            {
                                _baseLoader.PushBrokerAsync("basis/statistics/unit-day", _messagePublisher.RawCalculation.StatisticalUnitDay.ToJson()),
                                _baseLoader.PushBrokerAsync("parts/smart-meters/raw-data", _messagePublisher.ModbusHelper.MainElectricity.ToJson())
                            };
                            switch (_baseLoader.Profile.Controller.Type)
                            {
                                case MainDilation.Profile.TextController.HostType.Fanuc:
                                    tasks.Add(_baseLoader.PushBrokerAsync("parts/fanuc-controllers/raw-data", _messagePublisher.FocasHelper.Template.ToJson()));
                                    break;
                            }
                            await Task.WhenAll(tasks);
                        }
                    }
                    if (Histories.Any()) Histories.Clear();
                    _foundationPool.PushShellerBottom(DateTime.UtcNow);
                }
                catch (Exception e)
                {
                    _baseLoader.StorageEnabled = false;
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
            Log.Fatal(Menu.Title, nameof(EnvironmentLayout), new { e.Message, e.StackTrace });
        }
    }
    List<string> Histories { get; init; } = new();
}