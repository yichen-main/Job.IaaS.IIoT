namespace Station.Domain.Wrappers;

[Volo.Abp.DependencyInjection.Dependency(ServiceLifetime.Singleton)]
file sealed class TimeserieWrapper : ITimeserieWrapper
{
    public IOpcUaRegistrant OpcUaRegistrant => new OpcUaRegistrant(StructuralEngine, MainProfile);
    public IRootInformation RootInformation => new RootInformation(StructuralEngine, MainProfile);
    public IMaintenanceCycle MaintenanceCycle => new MaintenanceCycle(StructuralEngine, MainProfile);
    public ISpeedOdometer SpeedOdometer => new SpeedOdometer(StructuralEngine, MainProfile);
    public IThermalCompensation ThermalCompensation => new ThermalCompensation(StructuralEngine, MainProfile);
    public ICuttingFluidInformation CuttingFluidInformation => new CuttingFluidInformation(StructuralEngine, MainProfile);
    public IPumpMotorElectricity PumpMotorElectricity => new PumpMotorElectricity(StructuralEngine, MainProfile);
    public IElectricMeter ElectricMeter => new ElectricMeter(StructuralEngine, MainProfile);
    public IAttachedSensor AttachedSensor => new AttachedSensor(StructuralEngine, MainProfile);
    public required IMainProfile MainProfile { get; init; }
    public required IStructuralEngine StructuralEngine { get; init; }
}