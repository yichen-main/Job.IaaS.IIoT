namespace Station.Domain.Shared.Wrappers;
public interface ITimeserieWrapper
{
    enum BucketTag
    {
        [Description("base_enrollments")] Enrollment = 101,
        [Description("part_controllers")] Controller = 201,
        [Description("part_spindles")] Spindle = 202,
        [Description("part_water_tanks")] WaterTank = 203,
        [Description("tack_sensors")] Sensor = 301
    }
    IOpcUaRegistrant OpcUaRegistrant { get; }
    IRootInformation RootInformation { get; }
    IMaintenanceCycle MaintenanceCycle { get; }
    ISpeedOdometer SpeedOdometer { get; }
    IThermalCompensation ThermalCompensation { get; }
    ICuttingFluidInformation CuttingFluidInformation { get; }
    IPumpMotorElectricity PumpMotorElectricity { get; }
    IAttachedSensor AttachedSensor { get; }
    IElectricMeter ElectricMeter { get; }
}