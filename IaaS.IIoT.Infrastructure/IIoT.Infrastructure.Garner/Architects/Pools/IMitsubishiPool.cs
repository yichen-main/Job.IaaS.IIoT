using static Infrastructure.Garner.Architects.Pools.IMitsubishiPool;

namespace Infrastructure.Garner.Architects.Pools;
public interface IMitsubishiPool
{
    void Push(in AlarmGate value);
    void Push(in PartGate value);
    void Push(in FixtureGate value);
    void Push(in MaintenanceCycleData value);
    void Push(in SpindleSpeedOdometer[] values);
    void Push(in IRootInformation.Data.MachineStatusType value);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct AlarmGate
    {
        public byte DoorInterlock { get; init; }
        public byte SpindleJudgmentNoTool { get; init; }
        public byte RotaryTableOverheat { get; init; }
        public byte MotorOverload { get; init; }
        public byte AirPressureAlarm { get; init; }
        public byte SpindleCoolerAlarm { get; init; }
        public byte LubeAlarm { get; init; }
        public byte LubePressureAlarm { get; init; }
        public byte CoolantTankHigh { get; init; }
        public byte CoolantTankLow { get; init; }
    }

    [StructLayout(LayoutKind.Auto)]
    readonly record struct PartGate
    {
        public byte Ecomode { get; init; }
        public byte CuttingFluidMotor { get; init; }
        public byte ChassisCleanerMotor { get; init; }
        public byte ChipRemovalMotor { get; init; }
        public byte ChipRemovalBackwashMotor { get; init; }
        public byte CoolantThroughSpindleMotor { get; init; }
        public byte PumpMotor { get; init; }
    }

    [StructLayout(LayoutKind.Auto)]
    readonly record struct FixtureGate
    {
        public byte SpindleClamp { get; init; }
        public byte MachineClamp { get; init; }
        public byte MachineUnclamp { get; init; }
        public byte ToolUnclamp { get; init; }
        public byte ToolClamp { get; init; }
        public byte ToolManualRelaxation { get; init; }
        public byte ToolSetUpper { get; init; }
        public byte ToolSetLower { get; init; }
        public byte ToolMagazineCounter { get; init; }
        public byte ArmZeroPoint { get; init; }
        public byte ArmStopPoint { get; init; }
        public byte ArmPoint60Degrees { get; init; }
        public byte ArmPoint180Degrees { get; init; }
    }
    sealed class MaintenanceCycleData
    {
        public Meta[] Weeklies { get; init; } = Array.Empty<Meta>();
        public Meta[] Monthlies { get; init; } = Array.Empty<Meta>();
        public Meta[] Years { get; init; } = Array.Empty<Meta>();

        [StructLayout(LayoutKind.Auto)]
        public readonly record struct Meta
        {
            public required int SerialNo { get; init; }
            public required int CumulativeDay { get; init; }
        }
    }
    readonly record struct SpindleSpeedOdometer
    {
        public required int SerialNo { get; init; }
        public required int Hour { get; init; }
        public required int Minute { get; init; }
        public required int Second { get; init; }
        public required string Description { get; init; }
    }
    IRootInformation.Data.MachineStatusType MachineStatus { get; }
    AlarmGate Alarm { get; }
    PartGate Part { get; }
    FixtureGate Fixture { get; }
    MaintenanceCycleData MaintenanceCycle { get; }
    SpindleSpeedOdometer[] SpindleSpeedOdometers { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class MitsubishiPool : IMitsubishiPool
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