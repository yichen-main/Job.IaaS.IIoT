namespace Infrastructure.Pillbox.Wrappers;
public interface IPeripheralReader
{
    Task ComponentAsync(MainText mainText);
    Task FanucX64Async(string ip, ushort port);
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class PeripheralReader : IPeripheralReader
{
    readonly IFocasHelper _focasHelper;
    readonly IModbusHelper _modbusHelper;
    public PeripheralReader(IFocasHelper focasHelper, IModbusHelper modbusHelper)
    {
        _focasHelper = focasHelper;
        _modbusHelper = modbusHelper;
    }
    public async Task ComponentAsync(MainText mainText)
    {
        try
        {
            await _modbusHelper.ReadAsync(mainText.SerialEntry);
        }
        catch (Exception e)
        {
            Log.Error("[{0}] {1}", nameof(ComponentAsync), new { e.Message, e.StackTrace });
        }
    }

    public async Task FanucX64Async(string ip, ushort port)
    {
        try
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
        catch (Exception e)
        {
            Log.Error("[{0}] {1}", nameof(FanucX64Async), new { e.Message, e.StackTrace });
        }
    }
}