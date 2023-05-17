namespace Infrastructure.Garner.Timeseries.Parts.Controllers;
public interface ISpeedOdometer
{
    Task InsertAsync(Data[] datas);
    IDictionary<int, IAbstractPool.SpindleSpeedOdometerChartData.Detail> LatestTimeGroup(DateTime startTime, DateTime endTime);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct Data
    {
        public required int SerialNo { get; init; }
        public required int Hour { get; init; }
        public required int Minute { get; init; }
        public required int Second { get; init; }
    }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class SpeedOdometer : ISpeedOdometer
{
    readonly IInfluxExpert _influxExpert;
    public SpeedOdometer(IInfluxExpert influxExpert) => _influxExpert = influxExpert;
    public async Task InsertAsync(ISpeedOdometer.Data[] datas)
    {
        if (datas.Any()) await _influxExpert.WriteAsync(datas.Select(item => new Entity
        {
            SerialNo = item.SerialNo,
            Hour = item.Hour,
            Minute = item.Minute,
            Second = item.Second,
            Identifier = Identifier,
            Timestamp = DateTime.UtcNow
        }).ToArray(), Bucket);
    }
    public IDictionary<int, IAbstractPool.SpindleSpeedOdometerChartData.Detail> LatestTimeGroup(DateTime startTime, DateTime endTime)
    {
        return _influxExpert.Read<Entity>(Bucket, Identifier, startTime, endTime).GroupBy(item => item.SerialNo).ToDictionary(item => item.Key, item =>
        {
            int hour = default, minute = default, second = default;
            var entity = item.FirstOrDefault();
            if (entity is not null)
            {
                hour = entity.Hour;
                minute = entity.Minute;
                second = entity.Second;
            }
            return new IAbstractPool.SpindleSpeedOdometerChartData.Detail
            {
                Hour = hour,
                Minute = minute,
                Second = second,
                EventTime = endTime
            };
        }).OrderBy(item => item.Key).ToImmutableDictionary();
    }

    [Measurement("speed_odometers")]
    sealed class Entity : IInfluxExpert.MetaBase
    {
        [Column("serial_no")] public required int SerialNo { get; init; }
        [Column("hour")] public required int Hour { get; init; }
        [Column("minute")] public required int Minute { get; init; }
        [Column("second")] public required int Second { get; init; }
    }
    static string Identifier => nameof(SpeedOdometer).ToMd5().ToLower();
    static string Bucket => IInfluxExpert.BucketTag.Controller.GetDESC();
}