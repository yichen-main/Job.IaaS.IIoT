namespace Infrastructure.Garner.Timeseries.Parts.WaterTanks;
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
file sealed class PumpMotorElectricity : IPumpMotorElectricity
{
    readonly IInfluxExpert _influxExpert;
    public PumpMotorElectricity(IInfluxExpert influxExpert) => _influxExpert = influxExpert;
    public async Task InsertAsync(IPumpMotorElectricity.Data data) => await _influxExpert.WriteAsync(new Entity
    {
        AverageVoltage = data.AverageVoltage,
        AverageCurrent = data.AverageCurrent,
        ApparentPower = data.ApparentPower,
        Identifier = Identifier,
        Timestamp = DateTime.UtcNow
    }, Bucket);

    [Measurement("pump_motor_electricities")]
    sealed class Entity : IInfluxExpert.MetaBase
    {
        [Column("average_voltage")] public required float AverageVoltage { get; init; }
        [Column("average_current")] public required float AverageCurrent { get; init; }
        [Column("apparent_power")] public required float ApparentPower { get; init; }
    }
    static string Identifier => nameof(PumpMotorElectricity).ToMd5().ToLower();
    static string Bucket => IInfluxExpert.BucketTag.WaterTank.GetDESC();
}