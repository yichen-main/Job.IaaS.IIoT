namespace Station.Application.Operators.Bottoms;
internal sealed class ShellerBottom : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await StructuralEngine.Transport.StartAsync();
        while (await new PeriodicTimer(Menu.RefreshTime).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await MainProfile.RefreshAsync();
                if (MainProfile.Text is not null)
                {
                    Front.Grade = MainProfile.Text.Grade;
                    Local.CalibrationHour = MainProfile.Text.TimeZone switch
                    {
                        var item when item.Equals(TimeZoneType.ChinaStandardTime.GetDescription()) => (int)TimeZoneType.ChinaStandardTime,
                        _ => (int)TimeZoneType.UniversalTimeCoordinated
                    };
                    Local.Language = MainProfile.Text.Language;
                    foreach (ITimeserieWrapper.BucketTag item in Enum.GetValues(typeof(ITimeserieWrapper.BucketTag)))
                    {
                        var pool = MainProfile.Text.InfluxDB;
                        var name = item.GetType().GetDescription(item.ToString());
                        await StructuralEngine.InitialDataPoolAsync(pool.URL, pool.Organize, pool.UserName, pool.Password, name);
                    }
                    StructuralEngine.EnableStorage = true;
                }
                if (Histories.Any()) Histories.Clear();
                FoundationPool.PushShellerBottom(DateTime.UtcNow);
            }
            catch (Exception e)
            {
                StructuralEngine.EnableStorage = false;
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    HistoryEngine.Record(new IHistoryEngine.FavorerPayload
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
    public required IMainProfile MainProfile { get; init; }
    public required IHistoryEngine HistoryEngine { get; init; }
    public required IFoundationPool FoundationPool { get; init; }
    public required IStructuralEngine StructuralEngine { get; init; }
}