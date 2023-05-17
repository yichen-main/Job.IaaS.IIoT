namespace Infrastructure.Garner.Timeseries.Parts.Spindles;
public interface IThermalCompensation
{
    Task InsertAsync(Data data);
    IAbstractPool.SpindleThermalCompensationChartData ReadStatisticChart();

    [StructLayout(LayoutKind.Auto)]
    readonly record struct Data
    {
        public required float ThermalFirst { get; init; }
        public required float ThermalSecond { get; init; }
        public required float ThermalThird { get; init; }
        public required float ThermalFourth { get; init; }
        public required float ThermalFifth { get; init; }
        public required float CompensationX { get; init; }
        public required float CompensationY { get; init; }
        public required float CompensationZ { get; init; }
    }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class ThermalCompensation : IThermalCompensation
{
    readonly IInfluxExpert _influxExpert;
    readonly IDescriptiveStatistics _descriptiveStatistics;
    public ThermalCompensation(
        IInfluxExpert influxExpert,
        IDescriptiveStatistics descriptiveStatistics)
    {
        _influxExpert = influxExpert;
        _descriptiveStatistics = descriptiveStatistics;
    }
    public async Task InsertAsync(IThermalCompensation.Data data) => await _influxExpert.WriteAsync(new Entity
    {
        TemperatureFirst = data.ThermalFirst,
        TemperatureSecond = data.ThermalSecond,
        TemperatureThird = data.ThermalThird,
        TemperatureFourth = data.ThermalFourth,
        TemperatureFifth = data.ThermalFifth,
        CompensationX = data.CompensationX,
        CompensationY = data.CompensationY,
        CompensationZ = data.CompensationZ,
        Identifier = Identifier,
        Timestamp = DateTime.UtcNow
    }, Bucket);
    public IAbstractPool.SpindleThermalCompensationChartData ReadStatisticChart()
    {
        var endTime = DateTime.UtcNow.ToNowHour();
        var startTime = endTime.AddHours(Timeout.Infinite);
        var intervalTime = endTime.AddMinutes(Timeout.Infinite);
        var entities = _influxExpert.Read<Entity>(Bucket, Identifier, startTime, endTime).Where(item => item.TemperatureFirst is not 0);
        List<IAbstractPool.SpindleThermalCompensationChartData.AxisCompensation> results = new();
        while (startTime <= intervalTime)
        {
            var datas = entities.Where(item => item.Timestamp > intervalTime && item.Timestamp < intervalTime.AddMinutes(1));
            results.Add(new()
            {
                XAxis = (int)_descriptiveStatistics.Median(datas.Select(item => item.CompensationX).ToArray()),
                YAxis = (int)_descriptiveStatistics.Median(datas.Select(item => item.CompensationY).ToArray()),
                ZAxis = (int)_descriptiveStatistics.Median(datas.Select(item => item.CompensationZ).ToArray()),
                EventTime = intervalTime.AddMinutes(1)
            });
            intervalTime = intervalTime.AddMinutes(Timeout.Infinite);
        }
        return new()
        {
            TemperatureFirst = _descriptiveStatistics.Median(entities.Select(item => item.TemperatureFirst).ToArray()),
            TemperatureSecond = _descriptiveStatistics.Median(entities.Select(item => item.TemperatureSecond).ToArray()),
            TemperatureThird = _descriptiveStatistics.Median(entities.Select(item => item.TemperatureThird).ToArray()),
            TemperatureFourth = _descriptiveStatistics.Median(entities.Select(item => item.TemperatureFourth).ToArray()),
            TemperatureFifth = _descriptiveStatistics.Median(entities.Select(item => item.TemperatureFifth).ToArray()),
            RunCharts = results
        };
    }

    [Measurement("thermal_compensations")]
    sealed class Entity : IInfluxExpert.MetaBase
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
    static string Bucket => IInfluxExpert.BucketTag.Spindle.GetDESC();
}