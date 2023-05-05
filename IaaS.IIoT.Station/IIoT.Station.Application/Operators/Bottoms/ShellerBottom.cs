namespace Station.Application.Operators.Bottoms;
internal sealed class ShellerBottom : BackgroundService
{
    readonly IMainProfile _mainProfile;
    readonly IHistoryEngine _historyEngine;
    readonly IFoundationPool _foundationPool;
    readonly IStructuralEngine _structuralEngine;
    public ShellerBottom(
        IMainProfile mainProfile,
        IHistoryEngine historyEngine,
        IFoundationPool foundationPool,
        IStructuralEngine structuralEngine)
    {
        _mainProfile = mainProfile;
        _historyEngine = historyEngine;
        _foundationPool = foundationPool;
        _structuralEngine = structuralEngine;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _structuralEngine.Transport.StartAsync();
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
                    foreach (ITimeserieWrapper.BucketTag item in Enum.GetValues(typeof(ITimeserieWrapper.BucketTag)))
                    {
                        var pool = _mainProfile.Text.InfluxDB;
                        var name = item.GetType().GetDescription(item.ToString());
                        await _structuralEngine.InitialDataPoolAsync(pool.URL, pool.Organize, pool.UserName, pool.Password, name);
                    }
                    _structuralEngine.EnableStorage = true;
                }
                if (Histories.Any()) Histories.Clear();
                _foundationPool.PushShellerBottom(DateTime.UtcNow);
            }
            catch (Exception e)
            {
                _structuralEngine.EnableStorage = false;
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    _historyEngine.Record(new IHistoryEngine.FavorerPayload
                    {
                        Name = nameof(ShellerBottom),
                        Message = e.Message,
                        Source = e.Source
                    });
                }
            }
        }
    }
    internal required List<string> Histories { get; init; } = new();
}