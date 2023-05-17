namespace Infrastructure.Pillbox.Wrappers;
public interface IDataTransfer
{
    Task ControllerAsync();
    Task MerchandiseAsync();
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class DataTransfer : IDataTransfer
{
    readonly IBaseLoader _baseLoader;
    readonly IFocasHelper _focasHelper;
    readonly IModbusHelper _modbusHelper;
    public DataTransfer(IBaseLoader baseLoader, IFocasHelper focasHelper, IModbusHelper modbusHelper)
    {
        _baseLoader = baseLoader;
        _focasHelper = focasHelper;
        _modbusHelper = modbusHelper;
    }
    public async Task ControllerAsync()
    {
        try
        {
            if (_baseLoader.Profile is not null)
            {
                switch (_baseLoader.Profile.Controller.Type)
                {
                    case MainDilation.Profile.TextController.TextType.Fanuc:
                        {
                            _focasHelper.Open(_baseLoader.Profile.Controller.IP, Convert.ToUInt16(_baseLoader.Profile.Controller.Port));
                            if (_focasHelper.Enabled)
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
                                await _baseLoader.PushBrokerAsync("parts/controllers/data", new IFocasHelper.Template
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
                                }.ToJson());
                                _focasHelper.Close();
                            }
                        }
                        break;

                    case MainDilation.Profile.TextController.TextType.Siemens:
                        break;

                    case MainDilation.Profile.TextController.TextType.Mitsubishi:
                        break;

                    case MainDilation.Profile.TextController.TextType.Heidenhain:
                        break;
                }
                if (Histories.Any()) Histories.Clear();
            }
        }
        catch (Exception e)
        {
            if (!Histories.Contains(e.Message))
            {
                Histories.Add(e.Message);
                _baseLoader.Record(RecordType.MachineParts, new()
                {
                    Title = $"{nameof(DataTransfer)}.{nameof(ControllerAsync)}",
                    Name = "CNC Controller",
                    Message = e.Message
                });
            }
        }
    }
    public async Task MerchandiseAsync()
    {
        await _modbusHelper.ReadAsync();
    }
    List<string> Histories { get; init; } = new();
}