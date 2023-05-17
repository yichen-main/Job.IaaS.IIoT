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
file sealed class CuttingFluidInformation : ICuttingFluidInformation
{
    readonly IInfluxExpert _influxExpert;
    public CuttingFluidInformation(IInfluxExpert influxExpert) => _influxExpert = influxExpert;
    public async Task InsertAsync(ICuttingFluidInformation.Data data) => await _influxExpert.WriteAsync(new Entity
    {
        Temperature = data.Temperature,
        PowerOfHydrogen = data.PowerOfHydrogen,
        Identifier = Identifier,
        Timestamp = DateTime.UtcNow
    }, Bucket);

    [Measurement("cutting_fluid_informations")]
    sealed class Entity : IInfluxExpert.MetaBase
    {
        [Column("temperature")] public required float Temperature { get; init; }
        [Column("ph_value")] public required float PowerOfHydrogen { get; init; }
    }
    static string Identifier => nameof(CuttingFluidInformation).ToMd5().ToLower();
    static string Bucket => IInfluxExpert.BucketTag.WaterTank.GetDESC();
}