namespace Station.Domain.Shared.Functions.Pools;
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