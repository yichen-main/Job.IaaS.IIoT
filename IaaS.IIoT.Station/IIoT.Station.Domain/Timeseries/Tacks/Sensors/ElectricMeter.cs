namespace Station.Domain.Timeseries.Tacks.Sensors;
internal sealed class ElectricMeter : DepotEngine<ElectricMeter.Entity>, IElectricMeter
{
    readonly string _machineID;
    public ElectricMeter(IStructuralEngine engine, IMainProfile profile) : base(engine, profile)
    {
        _machineID = profile.Text?.MachineID ?? string.Empty;
    }
    public async Task InsertAsync(IElectricMeter.Data data) => await WriteAsync(new Entity
    {
        AverageVoltage = data.AverageVoltage,
        AverageCurrent = data.AverageCurrent,
        ApparentPower = data.ApparentPower,
        MachineID = _machineID,
        Identifier = Identifier,
        Timestamp = DateTime.UtcNow
    }, Bucket);
    public IEnumerable<IAbstractPool.ElectricityStatisticData> ReadElectricityStatistic()
    {
        var endTime = DateTime.UtcNow.ToNowHour();
        var startTime = endTime.AddDays(Timeout.Infinite);
        var intervalTime = endTime.AddHours(Timeout.Infinite);
        var entities = Read(Bucket, Identifier, startTime, endTime).Where(item => item.ApparentPower is not 0);
        List<IAbstractPool.ElectricityStatisticData> results = new();
        while (startTime <= intervalTime)
        {
            var datas = entities.Where(item => item.Timestamp > intervalTime && item.Timestamp < intervalTime.AddHours(1)).ToArray();
            results.Add(new()
            {
                KilowattHour = Median(datas.Select(item => item.ApparentPower).ToArray()),
                EventTime = intervalTime.AddHours(1)
            });
            intervalTime = intervalTime.AddHours(Timeout.Infinite);
        }
        return results;
    }

    [Measurement("electric_meters")]
    internal sealed class Entity : MetaBase
    {
        [Column("average_voltage")] public required float AverageVoltage { get; init; }
        [Column("average_current")] public required float AverageCurrent { get; init; }
        [Column("apparent_power")] public required float ApparentPower { get; init; }
    }
    static string Identifier => nameof(ElectricMeter).ToMd5().ToLower();
    static string Bucket => ITimeserieWrapper.BucketTag.Sensor.GetDescription();
}