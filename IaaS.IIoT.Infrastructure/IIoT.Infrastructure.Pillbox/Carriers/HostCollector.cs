namespace Infrastructure.Pillbox.Carriers;
internal sealed class HostCollector : BackgroundService
{
    readonly IMainProfile _mainProfile;
    readonly IControllerReader _controllerReader;
    public HostCollector(IMainProfile mainProfile, IControllerReader controllerReader)
    {
        _mainProfile = mainProfile;
        _controllerReader = controllerReader;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(TimeSpan.FromSeconds(1)).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                var text = _mainProfile.Text;
                if (text is not null)
                {
                    switch (text.Controller.Type)
                    {
                        case Core.Enactments.MainText.TextController.TextType.Fanuc:
                            await _controllerReader.ActionFanucX64(text.Controller.IP, Convert.ToUInt16(text.Controller.Port));
                            break;

                        case Core.Enactments.MainText.TextController.TextType.Siemens:
                            break;

                        case Core.Enactments.MainText.TextController.TextType.Mitsubishi:
                            break;

                        case Core.Enactments.MainText.TextController.TextType.Heidenhain:
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("[{0}] {1}", nameof(HostCollector), new { e.Message, e.StackTrace });
            }
        }
    }
}