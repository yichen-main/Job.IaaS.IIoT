namespace Station.Domain.Wrappers;

[Dependency(ServiceLifetime.Singleton)]
file sealed class TimeserieWrapper : ITimeserieWrapper
{
    readonly IMainProfile _mainProfile;
    readonly IStructuralEngine _structuralEngine;
    public TimeserieWrapper(IMainProfile mainProfile, IStructuralEngine structuralEngine)
    {
        _mainProfile = mainProfile;
        _structuralEngine = structuralEngine;
    }
    public IOpcUaRegistrant OpcUaRegistrant => new OpcUaRegistrant(_structuralEngine, _mainProfile);
    public IRootInformation RootInformation => new RootInformation(_structuralEngine, _mainProfile);
    public IMaintenanceCycle MaintenanceCycle => new MaintenanceCycle(_structuralEngine, _mainProfile);
    public ISpeedOdometer SpeedOdometer => new SpeedOdometer(_structuralEngine, _mainProfile);
    public IThermalCompensation ThermalCompensation => new ThermalCompensation(_structuralEngine, _mainProfile);
    public ICuttingFluidInformation CuttingFluidInformation => new CuttingFluidInformation(_structuralEngine, _mainProfile);
    public IPumpMotorElectricity PumpMotorElectricity => new PumpMotorElectricity(_structuralEngine, _mainProfile);
    public IElectricMeter ElectricMeter => new ElectricMeter(_structuralEngine, _mainProfile);
    public IAttachedSensor AttachedSensor => new AttachedSensor(_structuralEngine, _mainProfile);
}