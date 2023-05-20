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
        => (_baseLoader, _focasHelper, _modbusHelper) = (baseLoader, focasHelper, modbusHelper);
    public Task ControllerAsync()
    {
        try
        {
            if (_baseLoader.Profile is not null)
            {
                switch (_baseLoader.Profile.Controller.Type)
                {
                    case MainDilation.Profile.TextController.HostType.Fanuc:
                        _focasHelper.Open(_baseLoader.Profile.Controller.IP, Convert.ToUInt16(_baseLoader.Profile.Controller.Port));
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
                    Title = $"{nameof(DataTransfer)}.{nameof(ControllerAsync)}",
                    Name = "CNC Controller",
                    Message = e.Message
                });
            }
        }
        return Task.CompletedTask;
    }
    public async Task MerchandiseAsync()
    {
        await _modbusHelper.ReadAsync();
    }
    List<string> Histories { get; init; } = new();
}