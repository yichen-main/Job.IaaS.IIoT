namespace Station.Application.Operators.Attachs;
internal sealed class ModbusAttach : BackgroundService
{
    readonly IMainProfile _mainProfile;
    readonly IHistoryEngine _historyEngine;
    readonly IFoundationPool _foundationPool;
    public ModbusAttach(IMainProfile mainProfile, IHistoryEngine historyEngine, IFoundationPool foundationPool)
    {
        _mainProfile = mainProfile;
        _historyEngine = historyEngine;
        _foundationPool = foundationPool;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(Menu.RefreshTime).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                if (_mainProfile.Text is not null)
                {
                    if (_mainProfile.Text.DigiwinEAI.Enable)
                    {

                    }
                }
                if (Histories.Any()) Histories.Clear();
                _foundationPool.PushModbusAttach(DateTime.UtcNow);
            }
            catch (Exception e)
            {
                if (!Histories.Contains(e.Message))
                {
                    Histories.Add(e.Message);
                    _historyEngine.Record(new IHistoryEngine.FavorerPayload
                    {
                        Name = nameof(ModbusAttach),
                        Message = e.Message,
                        Source = e.Source
                    });
                }
            }
        }
    }
    internal required List<string> Histories { get; init; } = new();
}