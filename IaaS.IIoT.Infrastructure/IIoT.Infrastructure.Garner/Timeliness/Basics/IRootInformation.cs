using static Infrastructure.Garner.Timeliness.Basics.IRootInformation;

namespace Infrastructure.Garner.Timeliness.Basics;
public interface IRootInformation
{
    Task InsertAsync(Data data);
    IEnumerable<(byte status, DateTime time)> OneDayMachineStatusMinutes();
    public enum MachineStatus
    {
        Shutdown = 0,
        Idle = 1,
        Run = 2,
        Error = 3,
        Repair = 4,
        Maintenance = 5
    }

    [StructLayout(LayoutKind.Auto)]
    readonly record struct Data
    {
        public required MachineStatus Status { get; init; }
    }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class RootInformation : IRootInformation
{
    readonly IInfluxExpert _influxExpert;
    readonly IDescriptiveStatistics _descriptiveStatistics;
    public RootInformation(
        IInfluxExpert influxExpert,
        IDescriptiveStatistics descriptiveStatistics)
    {
        _influxExpert = influxExpert;
        _descriptiveStatistics = descriptiveStatistics;
    }
    public async Task InsertAsync(Data data) => await _influxExpert.WriteAsync(new Entity
    {
        Status = (byte)data.Status,
        Identifier = Identifier,
        Timestamp = DateTime.UtcNow
    }, Bucket);
    public IEnumerable<(byte status, DateTime time)> OneDayMachineStatusMinutes()
    {
        var endTime = DateTime.UtcNow.ToNowHour();
        var startTime = endTime.AddDays(Timeout.Infinite);
        var intervalTime = endTime.AddMinutes(Timeout.Infinite);
        var entities = _influxExpert.Read<Entity>(Bucket, Identifier, startTime, endTime);
        List<(byte status, DateTime time)> results = new();
        while (startTime <= intervalTime)
        {
            var result = entities.Where(item =>
            item.Timestamp > intervalTime && item.Timestamp < intervalTime.AddMinutes(1)).Select(item => item.Status).ToArray();
            results.Add((_descriptiveStatistics.Mode(result), intervalTime));
            intervalTime = intervalTime.AddMinutes(Timeout.Infinite);
        }
        return results;
    }

    [Measurement("root_informations")]
    sealed class Entity : IInfluxExpert.MetaBase
    {
        [Column("status")] public required byte Status { get; init; }
    }
    static string Identifier => nameof(RootInformation).ToMd5().ToLower();
    static string Bucket => IInfluxExpert.BucketTag.BaseMachine.GetDESC();
}