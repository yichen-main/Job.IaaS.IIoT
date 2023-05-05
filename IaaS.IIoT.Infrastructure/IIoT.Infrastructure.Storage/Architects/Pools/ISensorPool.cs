namespace Infrastructure.Storage.Architects.Pools;
public interface ISensorPool
{
    #region 1.Electrical Box
    void CacheElectricalBoxHumidity(float data);
    void CacheElectricalBoxTemperature(float data);
    void CacheElectricalBoxAverageVoltage(float data);
    void CacheElectricalBoxAverageCurrent(float data);
    void CacheElectricalBoxApparentPower(float data);
    (float value, DateTime time) GetElectricalBoxHumidity();
    (float value, DateTime time) GetElectricalBoxTemperature();
    (float value, DateTime time) GetElectricalBoxAverageVoltage();
    (float value, DateTime time) GetElectricalBoxAverageCurrent();
    (float value, DateTime time) GetElectricalBoxApparentPower();
    #endregion

    #region 2.Water Tank
    void CacheWaterTankTemperature(float data);
    void CacheCuttingFluidTemperature(float data);
    void CacheCuttingFluidPotentialOfHydrogen(float data);
    void CacheWaterPumpMotorAverageVoltage(float data);
    void CacheWaterPumpMotorAverageCurrent(float data);
    void CacheWaterPumpMotorApparentPower(float data);
    (float value, DateTime time) GetWaterTankTemperature();
    (float value, DateTime time) GetCuttingFluidTemperature();
    (float value, DateTime time) GetCuttingFluidPotentialOfHydrogen();
    (float value, DateTime time) GetWaterPumpMotorAverageVoltage();
    (float value, DateTime time) GetWaterPumpMotorAverageCurrent();
    (float value, DateTime time) GetWaterPumpMotorApparentPower();
    #endregion
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class SensorPool : ISensorPool
{
    #region 1.Electrical Box
    public void CacheElectricalBoxHumidity(float data) => ElectricalBoxes.AddOrUpdate(ElectricalBoxType.ElectricalBoxHumidity, (data, DateTime.UtcNow), (key, value) => (data, DateTime.UtcNow));
    public void CacheElectricalBoxTemperature(float data) => ElectricalBoxes.AddOrUpdate(ElectricalBoxType.ElectricalBoxTemperature, (data, DateTime.UtcNow), (key, value) => (data, DateTime.UtcNow));
    public void CacheElectricalBoxAverageVoltage(float data) => ElectricalBoxes.AddOrUpdate(ElectricalBoxType.ElectricalBoxAverageVoltage, (data, DateTime.UtcNow), (key, value) => (data, DateTime.UtcNow));
    public void CacheElectricalBoxAverageCurrent(float data) => ElectricalBoxes.AddOrUpdate(ElectricalBoxType.ElectricalBoxAverageCurrent, (data, DateTime.UtcNow), (key, value) => (data, DateTime.UtcNow));
    public void CacheElectricalBoxApparentPower(float data) => ElectricalBoxes.AddOrUpdate(ElectricalBoxType.ElectricalBoxApparentPower, (data, DateTime.UtcNow), (key, value) => (data, DateTime.UtcNow));
    public (float value, DateTime time) GetElectricalBoxHumidity()
    {
        if (ElectricalBoxes.TryGetValue(ElectricalBoxType.ElectricalBoxHumidity, out var value)) return value;
        return (default, default);
    }
    public (float value, DateTime time) GetElectricalBoxTemperature()
    {
        if (ElectricalBoxes.TryGetValue(ElectricalBoxType.ElectricalBoxTemperature, out var value)) return value;
        return (default, default);
    }
    public (float value, DateTime time) GetElectricalBoxAverageVoltage()
    {
        if (ElectricalBoxes.TryGetValue(ElectricalBoxType.ElectricalBoxAverageVoltage, out var value)) return value;
        return (default, default);
    }
    public (float value, DateTime time) GetElectricalBoxAverageCurrent()
    {
        if (ElectricalBoxes.TryGetValue(ElectricalBoxType.ElectricalBoxAverageCurrent, out var value)) return value;
        return (default, default);
    }
    public (float value, DateTime time) GetElectricalBoxApparentPower()
    {
        if (ElectricalBoxes.TryGetValue(ElectricalBoxType.ElectricalBoxApparentPower, out var value)) return value;
        return (default, default);
    }
    #endregion

    #region 2.Water Tank
    public void CacheWaterTankTemperature(float data) => WaterTanks.AddOrUpdate(WaterTankType.WaterTankTemperature, (data, DateTime.UtcNow), (key, value) => (data, DateTime.UtcNow));
    public void CacheCuttingFluidTemperature(float data) => WaterTanks.AddOrUpdate(WaterTankType.CuttingFluidTemperature, (data, DateTime.UtcNow), (key, value) => (data, DateTime.UtcNow));
    public void CacheCuttingFluidPotentialOfHydrogen(float data) => WaterTanks.AddOrUpdate(WaterTankType.CuttingFluidPotentialOfHydrogen, (data, DateTime.UtcNow), (key, value) => (data, DateTime.UtcNow));
    public void CacheWaterPumpMotorAverageVoltage(float data) => WaterTanks.AddOrUpdate(WaterTankType.WaterPumpMotorAverageVoltage, (data, DateTime.UtcNow), (key, value) => (data, DateTime.UtcNow));
    public void CacheWaterPumpMotorAverageCurrent(float data) => WaterTanks.AddOrUpdate(WaterTankType.WaterPumpMotorAverageCurrent, (data, DateTime.UtcNow), (key, value) => (data, DateTime.UtcNow));
    public void CacheWaterPumpMotorApparentPower(float data) => WaterTanks.AddOrUpdate(WaterTankType.WaterPumpMotorApparentPower, (data, DateTime.UtcNow), (key, value) => (data, DateTime.UtcNow));
    public (float value, DateTime time) GetWaterTankTemperature()
    {
        if (WaterTanks.TryGetValue(WaterTankType.WaterTankTemperature, out var value)) return value;
        return (default, default);
    }
    public (float value, DateTime time) GetCuttingFluidTemperature()
    {
        if (WaterTanks.TryGetValue(WaterTankType.CuttingFluidTemperature, out var value)) return value;
        return (default, default);
    }
    public (float value, DateTime time) GetCuttingFluidPotentialOfHydrogen()
    {
        if (WaterTanks.TryGetValue(WaterTankType.CuttingFluidPotentialOfHydrogen, out var value)) return value;
        return (default, default);
    }
    public (float value, DateTime time) GetWaterPumpMotorAverageVoltage()
    {
        if (WaterTanks.TryGetValue(WaterTankType.WaterPumpMotorAverageVoltage, out var value)) return value;
        return (default, default);
    }
    public (float value, DateTime time) GetWaterPumpMotorAverageCurrent()
    {
        if (WaterTanks.TryGetValue(WaterTankType.WaterPumpMotorAverageCurrent, out var value)) return value;
        return (default, default);
    }
    public (float value, DateTime time) GetWaterPumpMotorApparentPower()
    {
        if (WaterTanks.TryGetValue(WaterTankType.WaterPumpMotorApparentPower, out var value)) return value;
        return (default, default);
    }
    #endregion

    ConcurrentDictionary<ElectricalBoxType, (float value, DateTime eventTime)> ElectricalBoxes { get; init; } = new();
    ConcurrentDictionary<WaterTankType, (float value, DateTime eventTime)> WaterTanks { get; init; } = new();
}