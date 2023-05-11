namespace Infrastructure.Pillbox.Wrappers;
public interface IControllerReader
{
    ValueTask ActionFanucX64(string ip, ushort port);
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class ControllerReader : IControllerReader
{
    readonly IFocasHelper _focasHelper;
    public ControllerReader(IFocasHelper focasHelper)
    {
        _focasHelper = focasHelper;
    }
    public async ValueTask ActionFanucX64(string ip, ushort port)
    {
        _focasHelper.ConnectionStatus(_focasHelper.Open(ip, port));
        if (WindowsPass && _focasHelper.Enable)
        {
            var programName = _focasHelper.GetProgramName();
            var baseInformation = _focasHelper.GetBaseInformation();
            var jobInformation = _focasHelper.GetJobInformation();
            var programInformation = _focasHelper.GetProgramInformation();
            var spindleSpeed = _focasHelper.GetSpindleSpeed();
            var cuttingSpeed = _focasHelper.GetCuttingSpeed();
            var feedRate = _focasHelper.GetFeedRate();
            var turretCapacity = _focasHelper.GetTurretCapacity();
            var cuttingTime = _focasHelper.GetCuttingTime();
            var runTime = _focasHelper.GetRunTime();
            var bootTime = _focasHelper.GetBootTime();
            var coordinateAxes = _focasHelper.GetCoordinateAxes();
            var alarmMessage = _focasHelper.AlarmMessage();
            await _focasHelper.PushAsync(new()
            {
                AlarmMessage = alarmMessage,
                ProgramName = programName,
                FeedRate = feedRate,
                SpindleSpeed = spindleSpeed,
                CuttingSpeed = cuttingSpeed,
                CuttingTime = cuttingTime,
                BootTime = bootTime,
                RunTime = runTime,
                TurretCapacity = turretCapacity,
                BaseInformation = baseInformation,
                JobInformation = jobInformation,
                ProgramInformation = programInformation,
                Coordinates = coordinateAxes
            });
            _focasHelper.Close();
        }
    }
}