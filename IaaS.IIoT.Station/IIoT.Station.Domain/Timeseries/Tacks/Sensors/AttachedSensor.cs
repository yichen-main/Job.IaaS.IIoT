namespace Station.Domain.Timeseries.Tacks.Sensors;
internal sealed class AttachedSensor : DepotEngine<AttachedSensor.Entity>, IAttachedSensor
{
    readonly string _machineID;
    public AttachedSensor(IStructuralEngine engine, IMainProfile profile) : base(engine, profile)
    {
        _machineID = profile.Text?.MachineID ?? string.Empty;
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
    static string Bucket => ITimeserieWrapper.BucketTag.Sensor.GetDescription();
}