namespace Infrastructure.Garner.Wrappers;
public interface ITimelineWrapper
{
    IRootInformation RootInformation { get; }
    IOpcUaRegistrant OpcUaRegistrant { get; }
    IMaintenanceCycle MaintenanceCycle { get; }
    ISpeedOdometer SpeedOdometer { get; }
    IThermalCompensation ThermalCompensation { get; }
    ICuttingFluidInformation CuttingFluidInformation { get; }
    IPumpMotorElectricity PumpMotorElectricity { get; }
    IAttachedSensor AttachedSensor { get; }
    IElectricMeter ElectricMeter { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class TimelineWrapper(
        IRootInformation rootInformation,
        IOpcUaRegistrant opcUaRegistrant,
        IMaintenanceCycle maintenanceCycle,
        ISpeedOdometer speedOdometer,
        IThermalCompensation thermalCompensation,
        ICuttingFluidInformation cuttingFluidInformation,
        IPumpMotorElectricity pumpMotorElectricity,
        IElectricMeter electricMeter,
        IAttachedSensor attachedSensor) : ITimelineWrapper
{
    public required IRootInformation RootInformation { get; init; } = rootInformation;
    public required IOpcUaRegistrant OpcUaRegistrant { get; init; } = opcUaRegistrant;
    public required IMaintenanceCycle MaintenanceCycle { get; init; } = maintenanceCycle;
    public required ISpeedOdometer SpeedOdometer { get; init; } = speedOdometer;
    public required IThermalCompensation ThermalCompensation { get; init; } = thermalCompensation;
    public required ICuttingFluidInformation CuttingFluidInformation { get; init; } = cuttingFluidInformation;
    public required IPumpMotorElectricity PumpMotorElectricity { get; init; } = pumpMotorElectricity;
    public required IElectricMeter ElectricMeter { get; init; } = electricMeter;
    public required IAttachedSensor AttachedSensor { get; init; } = attachedSensor;
}