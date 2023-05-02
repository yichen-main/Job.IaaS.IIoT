namespace Infrastructure.Base.Utilities.Develops;
public static class SensorDevelop
{
    public enum ElectricalBoxType
    {
        ElectricalBoxHumidity = 1101,
        ElectricalBoxTemperature = 1102,
        ElectricalBoxAverageVoltage = 1111,
        ElectricalBoxAverageCurrent = 1112,
        ElectricalBoxApparentPower = 1113
    }
    public enum WaterTankType
    {
        WaterTankTemperature = 1201,
        CuttingFluidTemperature = 1202,
        CuttingFluidPotentialOfHydrogen = 1203,
        WaterPumpMotorAverageVoltage = 1211,
        WaterPumpMotorAverageCurrent = 1212,
        WaterPumpMotorApparentPower = 1213
    }
}