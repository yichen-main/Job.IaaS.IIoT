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
file sealed class TimelineWrapper : ITimelineWrapper
{
    public TimelineWrapper(
        IRootInformation rootInformation,
        IOpcUaRegistrant opcUaRegistrant,
        IMaintenanceCycle maintenanceCycle,
        ISpeedOdometer speedOdometer,
        IThermalCompensation thermalCompensation,
        ICuttingFluidInformation cuttingFluidInformation,
        IPumpMotorElectricity pumpMotorElectricity,
        IElectricMeter electricMeter,
        IAttachedSensor attachedSensor)
    {
        RootInformation = rootInformation;
        OpcUaRegistrant = opcUaRegistrant;
        MaintenanceCycle = maintenanceCycle;
        SpeedOdometer = speedOdometer;
        ThermalCompensation = thermalCompensation;
        CuttingFluidInformation = cuttingFluidInformation;
        PumpMotorElectricity = pumpMotorElectricity;
        ElectricMeter = electricMeter;
        AttachedSensor = attachedSensor;
    }
    public required IOpcUaRegistrant OpcUaRegistrant { get; init; }
    public required IRootInformation RootInformation { get; init; }
    public required IMaintenanceCycle MaintenanceCycle { get; init; }
    public required ISpeedOdometer SpeedOdometer { get; init; }
    public required IThermalCompensation ThermalCompensation { get; init; }
    public required ICuttingFluidInformation CuttingFluidInformation { get; init; }
    public required IPumpMotorElectricity PumpMotorElectricity { get; init; }
    public required IElectricMeter ElectricMeter { get; init; }
    public required IAttachedSensor AttachedSensor { get; init; }
}