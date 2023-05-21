namespace Infrastructure.Pillbox.Wrappers;
public interface IDataTransfer
{
    Task PushControllerAsync();
    Task PushMerchandiseAsync();
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class DataTransfer(IBaseLoader baseLoader, IFocasHelper focasHelper, IModbusHelper modbusHelper, ITimelineWrapper timelineWrapper) : IDataTransfer
{
    readonly IBaseLoader _baseLoader = baseLoader;
    readonly IFocasHelper _focasHelper = focasHelper;
    readonly IModbusHelper _modbusHelper = modbusHelper;
    readonly ITimelineWrapper _timelineWrapper = timelineWrapper;
    public Task PushControllerAsync()
    {
        try
        {
            if (_baseLoader.Profile is not null)
            {
                switch (_baseLoader.Profile.Controller.Type)
                {
                    case MainDilation.Profile.TextController.HostType.Fanuc:
                        _focasHelper.Open(_baseLoader.Profile.Controller.IP, Convert.ToUInt16(_baseLoader.Profile.Controller.Port));
                        _timelineWrapper.RootInformation.InsertAsync(new()
                        {
                            Status = _focasHelper.Information.OperatingState switch
                            {
                                IFocasHelper.OperatingState.Run => IRootInformation.MachineStatus.Run,
                                IFocasHelper.OperatingState.Alarm => IRootInformation.MachineStatus.Error,
                                _ => IRootInformation.MachineStatus.Idle
                            }
                        });
                        break;

                    case MainDilation.Profile.TextController.HostType.Siemens:
                        break;

                    case MainDilation.Profile.TextController.HostType.Mitsubishi:
                        break;

                    case MainDilation.Profile.TextController.HostType.Heidenhain:
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
                    Title = $"{nameof(DataTransfer)}.{nameof(PushControllerAsync)}",
                    Name = "CNC Controller",
                    Message = e.Message
                });
            }
        }
        return Task.CompletedTask;
    }
    public async Task PushMerchandiseAsync()
    {
        await _modbusHelper.ReadAsync();
    }
    List<string> Histories { get; init; } = new();
}