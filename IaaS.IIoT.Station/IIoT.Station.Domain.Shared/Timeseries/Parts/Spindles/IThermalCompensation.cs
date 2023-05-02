namespace Station.Domain.Shared.Timeseries.Parts.Spindles;
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