using static Infrastructure.Storage.Architects.Experts.IInfluxExpert;

namespace Infrastructure.Storage.Timeseries.Tacks.Sensors;
public interface IAttachedSensor
{
    Task InsertAsync(Data data);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct Data
    {
        public required float ElectricalBoxHumidity { get; init; }
        public required float ElectricalBoxTemperature { get; init; }
        public required float WaterTankTemperature { get; init; }
    }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class AttachedSensor : DepotDevelop<AttachedSensor.Entity>, IAttachedSensor
{
    readonly string _machineID;
    public AttachedSensor(IInfluxExpert influxExpert, IMainProfile mainProfile) : base(influxExpert, mainProfile)
    {
        _machineID = mainProfile.Text?.MachineID ?? string.Empty;
    }
    public async Task InsertAsync(IAttachedSensor.Data data) => await WriteAsync(new Entity
    {
        ElectricalBoxHumidity = data.ElectricalBoxHumidity,
        ElectricalBoxTemperature = data.ElectricalBoxTemperature,
        WaterTankTemperature = data.WaterTankTemperature,
        MachineID = _machineID,
        Identifier = Identifier,
        Timestamp = DateTime.UtcNow
    }, Bucket);

    [Measurement("attached_sensors")]
    internal sealed class Entity : MetaBase
    {
        [Column("electrical_box_humidity")] public required float ElectricalBoxHumidity { get; init; }
        [Column("electrical_box_temperature")] public required float ElectricalBoxTemperature { get; init; }
        [Column("water_tank_temperature")] public required float WaterTankTemperature { get; init; }
    }
    static string Identifier => nameof(AttachedSensor).ToMd5().ToLower();
    static string Bucket => BucketTag.Sensor.GetDescription();
}