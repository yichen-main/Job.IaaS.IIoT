using static Infrastructure.Storage.Architects.Pools.IAbstractPool;

namespace Infrastructure.Storage.Architects.Pools;
public interface IAbstractPool
{
    Task SetElectricityStatisticAsync();
    Task SetSpindleSpeedOdometerChartAsync();
    Task SetSpindleThermalCompensationChartAsync();

    [StructLayout(LayoutKind.Auto)]
    readonly record struct ElectricityStatisticData
    {
        public float KilowattHour { get; init; }
        public DateTime EventTime { get; init; }
    }
    sealed class SpindleThermalCompensationChartData
    {
        public float TemperatureFirst { get; init; }
        public float TemperatureSecond { get; init; }
        public float TemperatureThird { get; init; }
        public float TemperatureFourth { get; init; }
        public float TemperatureFifth { get; init; }
        public IEnumerable<AxisCompensation> RunCharts { get; init; } = Array.Empty<AxisCompensation>();

        [StructLayout(LayoutKind.Auto)]
        public readonly record struct AxisCompensation
        {
            public required int XAxis { get; init; }
            public required int YAxis { get; init; }
            public required int ZAxis { get; init; }
            public required DateTime EventTime { get; init; }
        }
    }
    sealed class SpindleSpeedOdometerChartData
    {
        public int SerialNo { get; init; }
        public int TotalHour { get; init; }
        public int TotalMinute { get; init; }
        public int TotalSecond { get; init; }
        public string Description { get; init; } = string.Empty;
        public IEnumerable<Detail> Details { get; init; } = Array.Empty<Detail>();

        [StructLayout(LayoutKind.Auto)]
        public readonly record struct Detail
        {
            public required int Hour { get; init; }
            public required int Minute { get; init; }
            public required int Second { get; init; }
            public required DateTime EventTime { get; init; }
        }
    }
    SpindleThermalCompensationChartData SpindleThermalCompensationChart { get; }
    IEnumerable<ElectricityStatisticData> ElectricityStatistics { get; }
    IEnumerable<SpindleSpeedOdometerChartData> SpindleSpeedOdometerCharts { get; }
}

[Dependency(ServiceLifetime.Singleton)]
file sealed class AbstractPool : IAbstractPool
{
    readonly IMitsubishiPool _mitsubishiPool;
    readonly ITimelineWrapper _timelineWrapper;
    public AbstractPool(IMitsubishiPool mitsubishiPool, ITimelineWrapper timelineWrapper)
    {
        _mitsubishiPool = mitsubishiPool;
        _timelineWrapper = timelineWrapper;
    }
    public async Task SetElectricityStatisticAsync() => await Task.Run(() => ElectricityStatistics = _timelineWrapper.ElectricMeter.ReadElectricityStatistic());
    public async Task SetSpindleSpeedOdometerChartAsync() => await Task.Run(() =>
    {
        var endTime = DateTime.UtcNow.ToNowHour();
        var startTime = endTime.AddDays(-7);
        var intervalTime = endTime.AddDays(Timeout.Infinite);
        List<(int serialNo, SpindleSpeedOdometerChartData.Detail detail)> details = new();
        while (startTime <= intervalTime)
        {
            foreach (var groups in _timelineWrapper.SpeedOdometer.LatestTimeGroup(intervalTime, intervalTime.AddDays(1)))
            {
                details.Add((groups.Key, groups.Value));
            }
            intervalTime = intervalTime.AddDays(Timeout.Infinite);
        }
        List<SpindleSpeedOdometerChartData> results = new();
        foreach (var detail in details.GroupBy(item => item.serialNo).ToDictionary(item => item.Key, item => item.Select(item => item.detail)))
        {
            var pool = _mitsubishiPool.SpindleSpeedOdometers.FirstOrDefault(item => item.SerialNo == detail.Key);
            results.Add(new()
            {
                SerialNo = pool.SerialNo,
                Description = pool.Description,
                TotalHour = pool.Hour,
                TotalMinute = pool.Minute,
                TotalSecond = pool.Second,
                Details = detail.Value
            });
        }
        foreach (var result in results)
        {

        }
        SpindleSpeedOdometerCharts = results;
    });
    public async Task SetSpindleThermalCompensationChartAsync() => await Task.Run(() => SpindleThermalCompensationChart = _timelineWrapper.ThermalCompensation.ReadStatisticChart());
    public SpindleThermalCompensationChartData SpindleThermalCompensationChart { get; private set; } = new();
    public IEnumerable<ElectricityStatisticData> ElectricityStatistics { get; private set; } = Array.Empty<ElectricityStatisticData>();
    public IEnumerable<SpindleSpeedOdometerChartData> SpindleSpeedOdometerCharts { get; private set; } = Array.Empty<SpindleSpeedOdometerChartData>();
}