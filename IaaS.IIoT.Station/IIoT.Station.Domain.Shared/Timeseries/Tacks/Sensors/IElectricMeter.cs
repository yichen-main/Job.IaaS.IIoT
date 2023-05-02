namespace Station.Domain.Shared.Timeseries.Tacks.Sensors;
public interface IElectricMeter
{
    Task InsertAsync(Data data);
    IEnumerable<IAbstractPool.ElectricityStatisticData> ReadElectricityStatistic();

    [StructLayout(LayoutKind.Auto)]
    readonly record struct Data
    {
        public required float AverageVoltage { get; init; }
        public required float AverageCurrent { get; init; }
        public required float ApparentPower { get; init; }
    }
}