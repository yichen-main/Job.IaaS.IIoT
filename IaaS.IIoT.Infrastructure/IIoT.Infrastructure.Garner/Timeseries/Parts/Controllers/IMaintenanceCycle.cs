namespace Infrastructure.Garner.Timeseries.Parts.Controllers;
public interface IMaintenanceCycle
{
    Task InsertAsync(Data[] datas);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct Data
    {
        public enum IntervalType
        {
            [Description("weekly")] Weekly = 1,
            [Description("monthly")] Monthly = 2,
            [Description("year")] Year = 3
        }
        public required IntervalType Interval { get; init; }
        public required int SerialNo { get; init; }
        public required int CumulativeDay { get; init; }
    }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class MaintenanceCycle : IMaintenanceCycle
{
    readonly IInfluxExpert _influxExpert;
    public MaintenanceCycle(IInfluxExpert influxExpert) => _influxExpert = influxExpert;
    public async Task InsertAsync(IMaintenanceCycle.Data[] datas)
    {
        if (datas.Any()) await _influxExpert.WriteAsync(datas.Select(item => new Entity
        {
            SerialNo = item.SerialNo,
            CumulativeDay = item.CumulativeDay,
            Interval = item.Interval.GetDESC(),
            Identifier = Identifier,
            Timestamp = DateTime.UtcNow
        }).ToArray(), Bucket);
    }

    [Measurement("maintenance_cycles")]
    sealed class Entity : IInfluxExpert.MetaBase
    {
        [Column("serial_no")] public required int SerialNo { get; init; }
        [Column("cumulative_day")] public required int CumulativeDay { get; init; }
        [Column("interval", IsTag = true)] public required string Interval { get; init; }
    }
    static string Identifier => nameof(MaintenanceCycle).ToMd5().ToLower();
    static string Bucket => IInfluxExpert.BucketTag.Controller.GetDESC();
}