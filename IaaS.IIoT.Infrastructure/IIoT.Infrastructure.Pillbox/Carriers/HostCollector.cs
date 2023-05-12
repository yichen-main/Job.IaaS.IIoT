namespace Infrastructure.Pillbox.Carriers;
internal sealed class HostCollector : BackgroundService
{
    readonly IMainProfile _mainProfile;
    readonly IPeripheralReader _peripheralReader;
    public HostCollector(IMainProfile mainProfile, IPeripheralReader peripheralReader)
    {
        _mainProfile = mainProfile;
        _peripheralReader = peripheralReader;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(TimeSpan.FromSeconds(1)).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                if (_mainProfile.Text is not null)
                {
                    List<Task> tasks = new();
                    switch (_mainProfile.Text.Controller.Type)
                    {
                        case MainText.TextController.TextType.Fanuc:
                            tasks.Add(_peripheralReader.FanucX64Async(_mainProfile.Text.Controller.IP, Convert.ToUInt16(_mainProfile.Text.Controller.Port)));
                            break;

                        case MainText.TextController.TextType.Siemens:
                            break;

                        case MainText.TextController.TextType.Mitsubishi:
                            break;

                        case MainText.TextController.TextType.Heidenhain:
                            break;

                        default:
                            break;
                    }
                    tasks.Add(_peripheralReader.ComponentAsync(_mainProfile.Text));
                    await Task.WhenAll(tasks);
                }
            }
            catch (Exception e)
            {
                Log.Error("[{0}] {1}", nameof(HostCollector), new { e.Message, e.StackTrace });
            }
        }
    }
}