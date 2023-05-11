using static Infrastructure.Garner.Architects.Experts.IInfluxExpert;

namespace Infrastructure.Garner.Timeseries.Parts.WaterTanks;
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

[Dependency(ServiceLifetime.Singleton)]
file sealed class CuttingFluidInformation : DepotDevelop<CuttingFluidInformation.Entity>, ICuttingFluidInformation
{
    readonly string _machineID;
    public CuttingFluidInformation(IInfluxExpert influxExpert, IMainProfile mainProfile) : base(influxExpert, mainProfile)
    {
        _machineID = mainProfile.Text?.MachineID ?? string.Empty;
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
    static string Bucket => BucketTag.WaterTank.GetDescription();
}