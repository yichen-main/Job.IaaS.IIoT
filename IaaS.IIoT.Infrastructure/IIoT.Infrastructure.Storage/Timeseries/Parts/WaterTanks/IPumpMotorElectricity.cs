using static Infrastructure.Storage.Architects.Experts.IInfluxExpert;

namespace Infrastructure.Storage.Timeseries.Parts.WaterTanks;
public interface IPumpMotorElectricity
{
    Task InsertAsync(Data data);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct Data
    {
        public required float AverageVoltage { get; init; }
        public required float AverageCurrent { get; init; }
        public required float ApparentPower { get; init; }
    }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class PumpMotorElectricity : DepotDevelop<PumpMotorElectricity.Entity>, IPumpMotorElectricity
{
    readonly string _machineID;
    public PumpMotorElectricity(IInfluxExpert influxExpert, IMainProfile mainProfile) : base(influxExpert, mainProfile)
    {
        _machineID = mainProfile.Text?.MachineID ?? string.Empty;
    }
    public async Task InsertAsync(IPumpMotorElectricity.Data data) => await WriteAsync(new Entity
    {
        AverageVoltage = data.AverageVoltage,
        AverageCurrent = data.AverageCurrent,
        ApparentPower = data.ApparentPower,
        MachineID = _machineID,
        Identifier = Identifier,
        Timestamp = DateTime.UtcNow
    }, Bucket);

    [Measurement("pump_motor_electricities")]
    internal sealed class Entity : MetaBase
    {
        [Column("average_voltage")] public required float AverageVoltage { get; init; }
        [Column("average_current")] public required float AverageCurrent { get; init; }
        [Column("apparent_power")] public required float ApparentPower { get; init; }
    }
    static string Identifier => nameof(PumpMotorElectricity).ToMd5().ToLower();
    static string Bucket => BucketTag.WaterTank.GetDescription();
}