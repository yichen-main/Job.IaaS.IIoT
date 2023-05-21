namespace Infrastructure.Garner.Timeseries.Tacks.Sensors;
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
file sealed class AttachedSensor(IInfluxExpert influxExpert) : IAttachedSensor
{
    readonly IInfluxExpert _influxExpert = influxExpert;
    public async Task InsertAsync(IAttachedSensor.Data data) => await _influxExpert.WriteAsync(new Entity
    {
        ElectricalBoxHumidity = data.ElectricalBoxHumidity,
        ElectricalBoxTemperature = data.ElectricalBoxTemperature,
        WaterTankTemperature = data.WaterTankTemperature,
        Identifier = Identifier,
        Timestamp = DateTime.UtcNow
    }, Bucket);

    [Measurement("attached_sensors")]
    sealed class Entity : IInfluxExpert.MetaBase
    {
        [Column("electrical_box_humidity")] public required float ElectricalBoxHumidity { get; init; }
        [Column("electrical_box_temperature")] public required float ElectricalBoxTemperature { get; init; }
        [Column("water_tank_temperature")] public required float WaterTankTemperature { get; init; }
    }
    static string Identifier => nameof(AttachedSensor).ToMd5().ToLower();
    static string Bucket => IInfluxExpert.BucketTag.Sensor.GetDESC();
}