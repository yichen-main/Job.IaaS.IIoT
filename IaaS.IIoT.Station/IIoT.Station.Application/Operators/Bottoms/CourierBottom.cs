namespace Station.Application.Operators.Bottoms;
internal sealed class CourierBottom : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(Menu.RefreshTime).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                if (MainProfile.Text is not null)
                {
                    if (MainProfile.Text.Controller.Enable)
                    {
                        switch (MainProfile.Text.Controller.Type)
                        {
                            case MainText.TextController.TextType.Mitsubishi:
                                await MitsubishiHost.CreateAsync((string)MainProfile.Text.Controller.IP, (int)MainProfile.Text.Controller.Port);
                                break;
                        }
                    }
                }
                if (Histories.Any()) Histories.Clear();
                FoundationPool.PushCourierBottom(DateTime.UtcNow);
            }
            catch (Exception e)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    HistoryEngine.Record(new IHistoryEngine.FavorerPayload
                    {
                        Name = nameof(CourierBottom),
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
    public required IMitsubishiHost MitsubishiHost { get; init; }
}