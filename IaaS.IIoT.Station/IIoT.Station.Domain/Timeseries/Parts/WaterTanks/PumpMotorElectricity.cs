namespace Station.Domain.Timeseries.Parts.WaterTanks;
internal sealed class PumpMotorElectricity : DepotEngine<PumpMotorElectricity.Entity>, IPumpMotorElectricity
{
    readonly string _machineID;
    public PumpMotorElectricity(IStructuralEngine engine, IMainProfile profile) : base(engine, profile)
    {
        _machineID = profile.Text?.MachineID ?? string.Empty;
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
    static string Bucket => ITimeserieWrapper.BucketTag.WaterTank.GetDescription();
}