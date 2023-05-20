namespace Platform.Application.Supports.Carriers;
internal sealed class EnvironmentLayout : BackgroundService
{
    readonly IBaseLoader _baseLoader;
    readonly IInitialService _initialService;
    readonly IMessagePublisher _messagePublisher;
    public EnvironmentLayout(IBaseLoader baseLoader, IInitialService initialService, IMessagePublisher messagePublisher)
        => (_baseLoader, _initialService, _messagePublisher) = (baseLoader, initialService, messagePublisher);
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _baseLoader.RefreshProfileAsync();
            await _baseLoader.Transport.StartAsync();
            await _initialService.CreateSwitchFileAsync();
            if (_baseLoader.Profile is not null) await _baseLoader.OverwriteProfileAsync(_baseLoader.Profile);
            while (await new PeriodicTimer(TimeSpan.FromSeconds(15)).WaitForNextTickAsync(stoppingToken))
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
                    }
                    if (Histories.Any()) Histories.Clear();
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
                            Name = nameof(Exception),
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