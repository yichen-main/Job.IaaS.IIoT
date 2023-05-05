namespace Infrastructure.Storage.Wrappers;
public interface ITimelineWrapper
{
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

[Dependency(ServiceLifetime.Singleton)]
file sealed class TimelineWrapper : ITimelineWrapper
{
    public TimelineWrapper(
        IOpcUaRegistrant opcUaRegistrant,
        IRootInformation rootInformation,
        IMaintenanceCycle maintenanceCycle,
        ISpeedOdometer speedOdometer,
        IThermalCompensation thermalCompensation,
        ICuttingFluidInformation cuttingFluidInformation,
        IPumpMotorElectricity pumpMotorElectricity,
        IElectricMeter electricMeter,
        IAttachedSensor attachedSensor)
    {
        OpcUaRegistrant = opcUaRegistrant;
        RootInformation = rootInformation;
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