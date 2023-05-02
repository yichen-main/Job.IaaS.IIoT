namespace Station.Domain.Timeseries.Parts.WaterTanks;
internal sealed class CuttingFluidInformation : DepotEngine<CuttingFluidInformation.Entity>, ICuttingFluidInformation
{
    readonly string _machineID;
    public CuttingFluidInformation(IStructuralEngine engine, IMainProfile profile) : base(engine, profile)
    {
        _machineID = profile.Text?.MachineID ?? string.Empty;
    }
    public async Task InsertAsync(ICuttingFluidInformation.Data data) => await WriteAsync(new Entity
    {
        Temperature = data.Temperature,
        PowerOfHydrogen = data.PowerOfHydrogen,
        MachineID = _machineID,
        Identifier = Identifier,
        Timestamp = DateTime.UtcNow
    }, Bucket);

    [Measurement("cutting_fluid_informations")]
    internal sealed class Entity : MetaBase
    {
        [Column("temperature")] public required float Temperature { get; init; }
        [Column("ph_value")] public required float PowerOfHydrogen { get; init; }
    }
    static string Identifier => nameof(CuttingFluidInformation).ToMd5().ToLower();
    static string Bucket => ITimeserieWrapper.BucketTag.WaterTank.GetDescription();
}