namespace Station.Domain.Shared.Accessors.Queues;
public interface IIcpdasQueue
{
    void PushElectricalBoxHumidity(in Meta meta);
    void PushElectricalBoxTemperature(in Meta meta);
    void PushElectricalBoxAverageVoltage(in Meta meta);
    void PushElectricalBoxAverageCurrent(in Meta meta);
    void PushElectricalBoxApparentPower(in Meta meta);
    void PushWaterTankTemperature(in Meta meta);
    void PushCuttingFluidTemperature(in Meta meta);
    void PushCuttingFluidPotentialOfHydrogen(in Meta meta);
    void PushWaterPumpMotorAverageVoltage(in Meta meta);
    void PushWaterPumpMotorAverageCurrent(in Meta meta);
    void PushWaterPumpMotorApparentPower(in Meta meta);
    readonly record struct Meta
    {
        [JsonPropertyName("nickname")] public required string Nickname { get; init; }
        [JsonPropertyName("value")] public required float Value { get; init; }
    }
}