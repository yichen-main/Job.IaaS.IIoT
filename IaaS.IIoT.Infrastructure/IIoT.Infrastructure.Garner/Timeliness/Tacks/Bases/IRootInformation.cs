using static Infrastructure.Garner.Timeliness.Tacks.Bases.IRootInformation;

namespace Infrastructure.Garner.Timeliness.Tacks.Bases;
public interface IRootInformation
{
    Task InsertAsync(Data data);
    Task<byte> MachineAvailabilityAsync();
    IAsyncEnumerable<(byte status, DateTime time)> OneDayMachineStatusMinutesAsync();
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
file sealed class RootInformation(IBaseLoader baseLoader, IInfluxExpert influxExpert, IDescriptiveStatistics descriptiveStatistics, IOverallEquipmentEffectiveness OEE) : IRootInformation
{
    readonly IBaseLoader _baseLoader = baseLoader;
    readonly IInfluxExpert _influxExpert = influxExpert;
    readonly IDescriptiveStatistics _descriptiveStatistics = descriptiveStatistics;
    readonly IOverallEquipmentEffectiveness _OEE = OEE;
    public async Task InsertAsync(Data data) => await _influxExpert.WriteAsync(new Entity
    {
        Status = (byte)data.Status,
        Identifier = Identifier,
        Timestamp = DateTime.UtcNow
    }, Bucket);
    public async Task<byte> MachineAvailabilityAsync()
    {
        byte availability = default;
        if (_baseLoader.Profile is not null)
        {
            var effectiveMinute = 0;
            List<(DateTime start, DateTime end)> periodTimes = new();
            foreach (var interval in _baseLoader.Profile.Formulation.WorkIntervals)
            {
                var endTime = Conversion(interval.EndMinute);
                var startTime = Conversion(interval.StartMinute);
                effectiveMinute += (int)endTime.Subtract(startTime).TotalMinutes;
                periodTimes.Add((startTime, endTime));
            }
            var currentTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm"));
            var entities = await _influxExpert.ReadAsync<Entity>(Bucket, Identifier, periodTimes[0].start, periodTimes[^1].end);
            var breakMinute = effectiveMinute;
            if (entities.Any())
            {
                foreach (var (start, end) in periodTimes)
                {
                    var result = entities.Where(item => item.Timestamp > start && item.Timestamp < end).Select(item => item.Status == (byte)MachineStatus.Run).ToArray();
                    if (result.Any()) breakMinute -= result.Length;
                }
            }
            availability = _OEE.PlanMachineAvailability(effectiveMinute, breakMinute);
        }
        static DateTime Conversion(string date) => DateTime.ParseExact(date, "HHmm", CultureInfo.InvariantCulture);
        return availability;
    }
    public async IAsyncEnumerable<(byte status, DateTime time)> OneDayMachineStatusMinutesAsync()
    {
        var endTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm"));
        var startTime = endTime.AddDays(Timeout.Infinite);
        var intervalTime = endTime.AddMinutes(Timeout.Infinite);
        var entities = await _influxExpert.ReadAsync<Entity>(Bucket, Identifier, startTime, endTime);
        while (startTime <= intervalTime)
        {
            var result = entities.Where(item => item.Timestamp > intervalTime && item.Timestamp < intervalTime.AddMinutes(1)).Select(item => item.Status).ToArray();
            yield return (_descriptiveStatistics.Mode(result), intervalTime);
            intervalTime = intervalTime.AddMinutes(Timeout.Infinite);
        }
    }

    [Measurement("root_informations")]
    sealed class Entity : IInfluxExpert.MetaBase
    {
        [Column("status")] public required byte Status { get; init; }
    }
    static string Identifier => nameof(RootInformation).ToMd5().ToLower();
    static string Bucket => IInfluxExpert.BucketTag.BaseMachine.GetDESC();
}