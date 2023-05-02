using static Station.Domain.Shared.Functions.Pools.IMitsubishiPool;

namespace Station.Domain.Functions.Pools;
internal sealed class MitsubishiPool : IMitsubishiPool
{
    public void Push(in PartGate value) => Part = value;
    public void Push(in FixtureGate value) => Fixture = value;
    public void Push(in AlarmGate value) => Alarm = value;
    public void Push(in MaintenanceCycleData value) => MaintenanceCycle = value;
    public void Push(in SpindleSpeedOdometer[] values) => SpindleSpeedOdometers = values;
    public void Push(in IRootInformation.Data.MachineStatusType value) => MachineStatus = value;
    public IRootInformation.Data.MachineStatusType MachineStatus { get; private set; }
    public AlarmGate Alarm { get; private set; } = new();
    public PartGate Part { get; private set; } = new();
    public FixtureGate Fixture { get; private set; } = new();
    public MaintenanceCycleData MaintenanceCycle { get; private set; } = new();
    public SpindleSpeedOdometer[] SpindleSpeedOdometers { get; private set; } = Array.Empty<SpindleSpeedOdometer>();
}