using static Infrastructure.Storage.Architects.Experts.IInfluxExpert;

namespace Infrastructure.Storage.Timeseries.Parts.Controllers;
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
file sealed class SpeedOdometer : DepotDevelop<SpeedOdometer.Entity>, ISpeedOdometer
{
    readonly string _machineID;
    public SpeedOdometer(IInfluxExpert influxExpert, IMainProfile mainProfile) : base(influxExpert, mainProfile)
    {
        _machineID = mainProfile.Text?.MachineID ?? string.Empty;
    }
    public async Task InsertAsync(ISpeedOdometer.Data[] datas)
    {
        if (datas.Any()) await WriteAsync(datas.Select(item => new Entity
        {
            SerialNo = item.SerialNo,
            Hour = item.Hour,
            Minute = item.Minute,
            Second = item.Second,
            MachineID = _machineID,
            Identifier = Identifier,
            Timestamp = DateTime.UtcNow
        }).ToArray(), Bucket);
    }
    public IDictionary<int, IAbstractPool.SpindleSpeedOdometerChartData.Detail> LatestTimeGroup(DateTime startTime, DateTime endTime)
    {
        return Read(Bucket, Identifier, startTime, endTime).GroupBy(item => item.SerialNo).ToDictionary(item => item.Key, item =>
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
    internal sealed class Entity : MetaBase
    {
        [Column("serial_no")] public required int SerialNo { get; init; }
        [Column("hour")] public required int Hour { get; init; }
        [Column("minute")] public required int Minute { get; init; }
        [Column("second")] public required int Second { get; init; }
    }
    static string Identifier => nameof(SpeedOdometer).ToMd5().ToLower();
    static string Bucket => BucketTag.Controller.GetDescription();
}