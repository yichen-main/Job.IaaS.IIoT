namespace Station.Application.Operators.Bottoms;
internal sealed class CourierBottom : BackgroundService
{
    readonly IMainProfile _mainProfile;
    readonly IHistoryEngine _historyEngine;
    readonly IFoundationPool _foundationPool;
    readonly IMitsubishiHost _mitsubishiHost;
    public CourierBottom(
        IMainProfile mainProfile,
        IHistoryEngine historyEngine,
        IFoundationPool foundationPool,
        IMitsubishiHost mitsubishiHost)
    {
        _mainProfile = mainProfile;
        _historyEngine = historyEngine;
        _foundationPool = foundationPool;
        _mitsubishiHost = mitsubishiHost;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(Menu.RefreshTime).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                if (_mainProfile.Text is not null)
                {
                    if (_mainProfile.Text.Controller.Enable)
                    {
                        switch (_mainProfile.Text.Controller.Type)
                        {
                            case MainText.TextController.TextType.Mitsubishi:
                                await _mitsubishiHost.CreateAsync(_mainProfile.Text.Controller.IP, _mainProfile.Text.Controller.Port);
                                break;
                        }
                    }
                }
                if (Histories.Any()) Histories.Clear();
                _foundationPool.PushCourierBottom(DateTime.UtcNow);
            }
            catch (Exception e)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    _historyEngine.Record(new IHistoryEngine.FavorerPayload
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
}