namespace Platform.Station.Hosts;
internal sealed class DoorHost : BackgroundService
{
    readonly IMainProfile _mainProfile;
    readonly IInfluxExpert _influxExpert;
    readonly IHistoryEngine _historyEngine;
    readonly IFoundationPool _foundationPool;
    readonly IInitialOperate _initialOperate;
    readonly IStructuralEngine _structuralEngine;
    readonly IStringLocalizer<Fielder> _fielderLocaliz;
    public DoorHost(
        IMainProfile mainProfile,
        IInfluxExpert influxExpert,
        IHistoryEngine historyEngine,
        IFoundationPool foundationPool,
        IInitialOperate initialOperate,
        IStructuralEngine structuralEngine,
        IStringLocalizer<Fielder> fielderLocaliz)
    {
        _mainProfile = mainProfile;
        _influxExpert = influxExpert;
        _historyEngine = historyEngine;
        _foundationPool = foundationPool;
        _initialOperate = initialOperate;
        _structuralEngine = structuralEngine;
        _fielderLocaliz = fielderLocaliz;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _mainProfile.RefreshAsync();
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
                            var pool = _mainProfile.Text.InfluxDB;
                            var name = item.GetType().GetDescription(item.ToString());
                            await _influxExpert.InitialDataPoolAsync(pool.URL, pool.Organize, pool.UserName, pool.Password, name);
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
                        _historyEngine.Record(new IHistoryEngine.FavorerPayload
                        {
                            Name = nameof(DoorHost),
                            Message = e.Message,
                            Source = e.Source
                        });
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(Menu.Title, nameof(DoorHost), new { e.Message });
        }
    }
    internal required List<string> Histories { get; init; } = new();
}