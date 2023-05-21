﻿namespace Platform.Application.Contract.Wrappers;
public interface IMessagePublisher
{
    Task BasicInformationAsync();
    Task PartInformationAsync();
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class MessagePublisher(IBaseLoader baseLoader, IFocasHelper focasHelper, IModbusHelper modbusHelper, IRawCalculation rawCalculation) : IMessagePublisher
{
    readonly IBaseLoader _baseLoader = baseLoader;
    readonly IFocasHelper _focasHelper = focasHelper;
    readonly IModbusHelper _modbusHelper = modbusHelper;
    readonly IRawCalculation _rawCalculation = rawCalculation;
    public async Task BasicInformationAsync()
    {
        var tag = "bases";
        await SendAsync($"{tag}/statistics/unit-day", _rawCalculation.StatisticalUnitDay.ToJson());
        await SendAsync($"{tag}/electricities/raw-data", _modbusHelper.ElectricityEnergy.ToJson());
    }
    public async Task PartInformationAsync()
    {
        if (_baseLoader.Profile is not null)
        {
            var tag = "parts";
            switch (_baseLoader.Profile.Controller.Type)
            {
                case MainDilation.Profile.TextController.HostType.Fanuc:
                    await SendAsync($"{tag}/controllers/fanuc-information", _focasHelper.Information.ToJson());
                    await SendAsync($"{tag}/controllers/fanuc-spindle", _focasHelper.PartSpindle.ToJson());
                    break;
            }
        }
    }
    async ValueTask SendAsync(string path, string message)
    {
        if (_baseLoader.Transport.IsStarted) await _baseLoader.Transport.InjectApplicationMessage(
        new(new()
        {
            Topic = path,
            Retain = true,
            QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
            PayloadSegment = Encoding.UTF8.GetBytes(message)
        })
        { SenderClientId = path.ToMd5() });
    }
}