namespace Infrastructure.Garner.Wrappers;
public interface ITimelineWrapper
{
    IRootInformation RootInformation { get; }
    IOpcUaRegistrant OpcUaRegistrant { get; }
    IMaintenanceCycle MaintenanceCycle { get; }
    ISpeedOdometer SpeedOdometer { get; }
    ICuttingFluidInformation CuttingFluidInformation { get; }
    IPumpMotorElectricity PumpMotorElectricity { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class TimelineWrapper(
    IRootInformation rootInformation,
    IOpcUaRegistrant opcUaRegistrant,
    IMaintenanceCycle maintenanceCycle,
    ISpeedOdometer speedOdometer,
    ICuttingFluidInformation cuttingFluidInformation,
    IPumpMotorElectricity pumpMotorElectricity) : ITimelineWrapper
{
    public required IRootInformation RootInformation { get; init; } = rootInformation;
    public required IOpcUaRegistrant OpcUaRegistrant { get; init; } = opcUaRegistrant;
    public required IMaintenanceCycle MaintenanceCycle { get; init; } = maintenanceCycle;
    public required ISpeedOdometer SpeedOdometer { get; init; } = speedOdometer;
    public required ICuttingFluidInformation CuttingFluidInformation { get; init; } = cuttingFluidInformation;
    public required IPumpMotorElectricity PumpMotorElectricity { get; init; } = pumpMotorElectricity;
}