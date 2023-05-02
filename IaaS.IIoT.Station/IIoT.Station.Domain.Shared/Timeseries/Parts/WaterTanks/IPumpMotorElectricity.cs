namespace Station.Domain.Shared.Timeseries.Parts.WaterTanks;
public interface IPumpMotorElectricity
{
    Task InsertAsync(Data data);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct Data
    {
        public required float AverageVoltage { get; init; }
        public required float AverageCurrent { get; init; }
        public required float ApparentPower { get; init; }
    }
}