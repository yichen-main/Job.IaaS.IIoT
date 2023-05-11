namespace Platform.Application.Carriers.Supports;
internal sealed class NavelCarrier : BackgroundService
{
    readonly IMainProfile _mainProfile;
    readonly IInfluxExpert _influxExpert;
    readonly IHistoryRecorder _historyEngine;
    readonly IFoundationPool _foundationPool;
    public NavelCarrier(
        IMainProfile mainProfile,
        IInfluxExpert influxExpert,
        IHistoryRecorder historyEngine,
        IFoundationPool foundationPool)
    {
        _mainProfile = mainProfile;
        _influxExpert = influxExpert;
        _historyEngine = historyEngine;
        _foundationPool = foundationPool;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _mainProfile.RefreshAsync();
            await _mainProfile.Transport.StartAsync();
            if (_mainProfile.Text is not null) await _mainProfile.OverwriteAsync(_mainProfile.Text);
            while (await new PeriodicTimer(Menu.RefreshTime).WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await _mainProfile.RefreshAsync();
                    if (_mainProfile.Text is not null)
                    {
                        Front.Grade = _mainProfile.Text.Grade;
                        Local.CalibrationHour = _mainProfile.Text.TimeZone switch
                        {
                            var item when item.Equals(TimeZoneType.ChinaStandardTime.GetDescription()) => (int)TimeZoneType.ChinaStandardTime,
                            _ => (int)TimeZoneType.UniversalTimeCoordinated
                        };
                        Local.Language = _mainProfile.Text.Language;
                        foreach (IInfluxExpert.BucketTag item in Enum.GetValues(typeof(IInfluxExpert.BucketTag)))
                        {
                            var name = item.GetType().GetDescription(item.ToString());
                            await _influxExpert.InitialDataPoolAsync(name);
                        }
                        _influxExpert.EnableStorage = true;
                    }
                    if (Histories.Any()) Histories.Clear();
                    _foundationPool.PushShellerBottom(DateTime.UtcNow);
                }
                catch (Exception e)
                {
                    _influxExpert.EnableStorage = false;
                    if (!Histories.Contains(e.Message))
                    {
                        Histories.Add(e.Message);
                        _historyEngine.Record(new IHistoryRecorder.FavorerPayload
                        {
                            Name = nameof(NavelCarrier),
                            Message = e.Message,
                            Source = e.Source
                        });
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(Menu.Title, nameof(NavelCarrier), new { e.Message });
        }
    }
    internal required List<string> Histories { get; init; } = new();
}