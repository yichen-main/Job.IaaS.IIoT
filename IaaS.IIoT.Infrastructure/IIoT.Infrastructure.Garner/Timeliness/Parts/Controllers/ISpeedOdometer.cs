namespace Infrastructure.Garner.Timeliness.Parts.Controllers;
public interface ISpeedOdometer
{
    Task InsertAsync(Data[] datas);

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
file sealed class SpeedOdometer(IInfluxExpert influxExpert) : ISpeedOdometer
{
    readonly IInfluxExpert _influxExpert = influxExpert;
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