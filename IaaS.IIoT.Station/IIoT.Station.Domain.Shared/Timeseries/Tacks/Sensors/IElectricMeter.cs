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
    enum Field
    {
        [Description("Vrms")] VoltageRootMeanSquare,
        [Description("Irms")] CurrentRootMeanSquare,
        [Description("kW")] ActivePower,
        [Description("kWh")] ActiveEnergy,
        [Description("kVA")] ApparentPower,
        [Description("kVAh")] ApparentEnergy,
        [Description("kVAR")] ReactivePower,
        [Description("kVARh")] ReactiveEnergy,
        [Description("PF")] PowerFactor,
        [Description("Hz")] Frequency
    }
}