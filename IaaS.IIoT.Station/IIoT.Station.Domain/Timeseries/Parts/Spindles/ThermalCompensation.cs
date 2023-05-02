namespace Station.Domain.Timeseries.Parts.Spindles;
internal sealed class ThermalCompensation : DepotEngine<ThermalCompensation.Entity>, IThermalCompensation
{
    readonly string _machineID;
    public ThermalCompensation(IStructuralEngine engine, IMainProfile profile) : base(engine, profile)
    {
        _machineID = profile.Text?.MachineID ?? string.Empty;
    }
    public async Task InsertAsync(IThermalCompensation.Data data) => await WriteAsync(new Entity
    {
        TemperatureFirst = data.ThermalFirst,
        TemperatureSecond = data.ThermalSecond,
        TemperatureThird = data.ThermalThird,
        TemperatureFourth = data.ThermalFourth,
        TemperatureFifth = data.ThermalFifth,
        CompensationX = data.CompensationX,
        CompensationY = data.CompensationY,
        CompensationZ = data.CompensationZ,
        MachineID = _machineID,
        Identifier = Identifier,
        Timestamp = DateTime.UtcNow
    }, Bucket);
    public IAbstractPool.SpindleThermalCompensationChartData ReadStatisticChart()
    {
        var endTime = DateTime.UtcNow.ToNowHour();
        var startTime = endTime.AddHours(Timeout.Infinite);
        var intervalTime = endTime.AddMinutes(Timeout.Infinite);
        var entities = Read(Bucket, Identifier, startTime, endTime).Where(item => item.TemperatureFirst is not 0);
        List<IAbstractPool.SpindleThermalCompensationChartData.AxisCompensation> results = new();
        while (startTime <= intervalTime)
        {
            var datas = entities.Where(item => item.Timestamp > intervalTime && item.Timestamp < intervalTime.AddMinutes(1));
            results.Add(new()
            {
                XAxis = (int)Median(datas.Select(item => item.CompensationX).ToArray()),
                YAxis = (int)Median(datas.Select(item => item.CompensationY).ToArray()),
                ZAxis = (int)Median(datas.Select(item => item.CompensationZ).ToArray()),
                EventTime = intervalTime.AddMinutes(1)
            });
            intervalTime = intervalTime.AddMinutes(Timeout.Infinite);
        }
        return new()
        {
            TemperatureFirst = Median(entities.Select(item => item.TemperatureFirst).ToArray()),
            TemperatureSecond = Median(entities.Select(item => item.TemperatureSecond).ToArray()),
            TemperatureThird = Median(entities.Select(item => item.TemperatureThird).ToArray()),
            TemperatureFourth = Median(entities.Select(item => item.TemperatureFourth).ToArray()),
            TemperatureFifth = Median(entities.Select(item => item.TemperatureFifth).ToArray()),
            RunCharts = results
        };
    }

    [Measurement("thermal_compensations")]
    internal sealed class Entity : MetaBase
    {
        [Column("temperature_first")] public required float TemperatureFirst { get; init; }
        [Column("temperature_second")] public required float TemperatureSecond { get; init; }
        [Column("temperature_third")] public required float TemperatureThird { get; init; }
        [Column("temperature_fourth")] public required float TemperatureFourth { get; init; }
        [Column("temperature_fifth")] public required float TemperatureFifth { get; init; }
        [Column("compensation_x")] public required float CompensationX { get; init; }
        [Column("compensation_y")] public required float CompensationY { get; init; }
        [Column("compensation_z")] public required float CompensationZ { get; init; }
    }
    static string Identifier => nameof(ThermalCompensation).ToMd5().ToLower();
    static string Bucket => ITimeserieWrapper.BucketTag.Spindle.GetDescription();
}