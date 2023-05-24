namespace Platform.Application.Contract.Wrappers;
public interface IMessagePublisher
{
    Task BaseGroupAsync();
    Task PartGroupAsync();
    CancellationToken Token { get; set; }
    IBaseLoader BaseLoader { get; }
    IFocasHelper FocasHelper { get; }
    IModbusHelper ModbusHelper { get; }
    IRawCalculation RawCalculation { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class MessagePublisher(IBaseLoader baseLoader, IFocasHelper focasHelper, IModbusHelper modbusHelper, IRawCalculation rawCalculation) : IMessagePublisher
{
    public async Task BaseGroupAsync()
    {
        var tag = "bases";
        if (ModbusHelper.Electricity.AverageVoltage is not 0) await SendAsync($"{tag}/electricities/raw-data", ModbusHelper.Electricity.ToJson());
        if (RawCalculation.StatisticalUnitDay.RunChartMinutes is not null) await SendAsync($"{tag}/statistics/unit-day", RawCalculation.StatisticalUnitDay.ToJson());
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
        { SenderClientId = path.ToMd5() }, Token);
    }
    public CancellationToken Token { get; set; }
    public required IBaseLoader BaseLoader { get; init; } = baseLoader;
    public required IFocasHelper FocasHelper { get; init; } = focasHelper;
    public required IModbusHelper ModbusHelper { get; init; } = modbusHelper;
    public required IRawCalculation RawCalculation { get; init; } = rawCalculation;
}