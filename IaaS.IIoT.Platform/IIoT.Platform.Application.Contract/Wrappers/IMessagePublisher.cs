namespace Platform.Application.Contract.Wrappers;
public interface IMessagePublisher
{
    Task BaseGroupAsync();
    Task PartGroupAsync();
    public IBaseLoader BaseLoader { get; }
    public IFocasHelper FocasHelper { get; }
    public IModbusHelper ModbusHelper { get; }
    public IRawCalculation RawCalculation { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class MessagePublisher(IBaseLoader baseLoader, IFocasHelper focasHelper, IModbusHelper modbusHelper, IRawCalculation rawCalculation) : IMessagePublisher
{
    public async Task BaseGroupAsync()
    {
        var tag = "bases";
        if (ModbusHelper.ElectricityEnergy.AverageVoltage is not 0) await SendAsync($"{tag}/electricities/raw-data", ModbusHelper.ElectricityEnergy.ToJson());
        if (RawCalculation.StatisticalUnitDay.RunChartMinutes.Any()) await SendAsync($"{tag}/statistics/unit-day", RawCalculation.StatisticalUnitDay.ToJson());
    }
    public async Task PartGroupAsync()
    {
        if (BaseLoader.Profile is not null)
        {
            var tag = "parts";
            switch (BaseLoader.Profile.Controller.Type)
            {
                case MainDilation.Profile.TextController.HostType.Fanuc:
                    if (FocasHelper.Connected)
                    {
                        await SendAsync($"{tag}/controllers/fanuc-information", FocasHelper.Information.ToJson());
                        await SendAsync($"{tag}/controllers/fanuc-spindle", FocasHelper.PartSpindle.ToJson());
                    }
                    break;
            }
        }
    }
    async ValueTask SendAsync(string path, string message)
    {
        if (BaseLoader.Transport.IsStarted) await BaseLoader.Transport.InjectApplicationMessage(
        new(new()
        {
            Topic = path,
            Retain = true,
            QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
            PayloadSegment = Encoding.UTF8.GetBytes(message)
        })
        { SenderClientId = path.ToMd5() });
    }
    public required IBaseLoader BaseLoader { get; init; } = baseLoader;
    public required IFocasHelper FocasHelper { get; init; } = focasHelper;
    public required IModbusHelper ModbusHelper { get; init; } = modbusHelper;
    public required IRawCalculation RawCalculation { get; init; } = rawCalculation;
}