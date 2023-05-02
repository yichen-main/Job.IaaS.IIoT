namespace Station.Domain.Shared.Timeseries.Parts.WaterTanks;
public interface ICuttingFluidInformation
{
    Task InsertAsync(Data data);

    [StructLayout(LayoutKind.Auto)]
    readonly record struct Data
    {
        public required float Temperature { get; init; }
        public required float PowerOfHydrogen { get; init; }
    }
}