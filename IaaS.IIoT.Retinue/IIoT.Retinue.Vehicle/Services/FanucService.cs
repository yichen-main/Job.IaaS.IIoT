namespace Retinue.Vehicle.Services;
internal sealed class FanucService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await new PeriodicTimer(TimeSpan.FromSeconds(1)).WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                FocasHelper.ConnectionStatus(FocasHelper.Open("192.168.26.126", 8193));
                if (FocasHelper.Enable)
                {
                    var systemInfo = FocasHelper.GetSystemInfo();
                    var a = $"�D�b��:{systemInfo.axes[0]}, �Q������A�b��:{systemInfo.axes[1]}";

                    FocasHelper.Close();
                }
            }
            catch (Exception e)
            {
                Log.Error("[{0}] {1}", nameof(FanucService), new { e.Message, e.StackTrace });
            }
        }
    }
    public required IFocasHelper FocasHelper { get; init; }
}