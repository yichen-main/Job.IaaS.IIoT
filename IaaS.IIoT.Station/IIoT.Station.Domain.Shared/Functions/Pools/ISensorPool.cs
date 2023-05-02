namespace Station.Domain.Shared.Functions.Pools;
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