using static Station.Domain.Shared.Accessors.Queues.IIcpdasQueue;

namespace Station.Domain.Accessors.Queues;
internal sealed class IcpdasQueue : IIcpdasQueue
{
    readonly ISensorPool _sensorPool;
    public IcpdasQueue(ISensorPool sensorPool) => _sensorPool = sensorPool;
    public void PushElectricalBoxHumidity(in Meta meta) => _sensorPool.CacheElectricalBoxHumidity(meta.Value);
    public void PushElectricalBoxTemperature(in Meta meta) => _sensorPool.CacheElectricalBoxTemperature(meta.Value);
    public void PushElectricalBoxAverageVoltage(in Meta meta) => _sensorPool.CacheElectricalBoxAverageVoltage(meta.Value);
    public void PushElectricalBoxAverageCurrent(in Meta meta) => _sensorPool.CacheElectricalBoxAverageCurrent(meta.Value);
    public void PushElectricalBoxApparentPower(in Meta meta) => _sensorPool.CacheElectricalBoxApparentPower(meta.Value);
    public void PushWaterTankTemperature(in Meta meta) => _sensorPool.CacheWaterTankTemperature(meta.Value);
    public void PushCuttingFluidTemperature(in Meta meta) => _sensorPool.CacheCuttingFluidTemperature(meta.Value);
    public void PushCuttingFluidPotentialOfHydrogen(in Meta meta) => _sensorPool.CacheCuttingFluidPotentialOfHydrogen(meta.Value);
    public void PushWaterPumpMotorAverageVoltage(in Meta meta) => _sensorPool.CacheWaterPumpMotorAverageVoltage(meta.Value);
    public void PushWaterPumpMotorAverageCurrent(in Meta meta) => _sensorPool.CacheWaterPumpMotorAverageCurrent(meta.Value);
    public void PushWaterPumpMotorApparentPower(in Meta meta) => _sensorPool.CacheWaterPumpMotorApparentPower(meta.Value);
}